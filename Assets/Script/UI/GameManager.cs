using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    GameObject player;
    public TextMeshProUGUI InGame_Scoretext;

    [HideInInspector]
    private float playerStartX;

    public float worldSpeed;

    [HideInInspector]
    public float speedMultiplier = 1f;
    public bool playerBlocked;

    public enum GameState
    {
        Start,
        Setting,
        Running,
        Paused,
        GameOver,
        Language,
        Update,
    }

    // public TMP_Dropdown soundDropdown;
    // public TMP_Dropdown musicDropdown;
    public GameObject gameSettingScreen;
    public GameState currentState;
    public GameObject gameStartScreen;
    public GameObject gameOverScreen;
    public GameObject pauseGameScreen;
    public GameObject inGameScreen;
    public GameObject UpdateScreen;
    public TextMeshProUGUI GameScoreText;
    public TextMeshProUGUI GameOverScoreText;
    public GameObject LanguageScreen;
    public GameObject SpawnManagerObject;
    public GameObject NoAdGameObject;
    public GameObject Prefeb_1;
    public string currentLanguage;

    public GameObject englishCheck;
    public GameObject portugueseCheck;
    public GameObject spanishCheck;
    public GameObject frenchCheck;

    [HideInInspector]
    public float score;

    [HideInInspector]
    public float extraScore = 0;

    [HideInInspector]
    public float scoreRate = 10f;

    [HideInInspector]
    public static bool restartFromGameOver = false;

    [HideInInspector]
    // public static bool restartFromMainMenu = false;

    public TextMeshProUGUI smashText;
    public TextMeshProUGUI executedText;

    [HideInInspector]
    private float smashTextDuration = 0.5f;

    [HideInInspector]
    private int smashCombo = 0;

    [HideInInspector]
    private int ExecutedCombo = 0;

    [HideInInspector]
    private float comboResetTime = 0.5f;

    [HideInInspector]
    private Coroutine comboResetCoroutine;

    [HideInInspector]
    private Coroutine executedResetCoroutine;

    public float maxEnergy = 0f;
    public float currentEnergy;

    [HideInInspector]
    public int overallHighScore;
    public TextMeshProUGUI overallHighScoreText;
    public TextMeshProUGUI gameLanguageText;

    [HideInInspector]
    private bool isPaused = false;

    [SerializeField]
    private AudioSource musicSource;

    [SerializeField]
    private AudioSource sfxSource;

    [SerializeField]
    private AudioClip dashSound;

    [SerializeField]
    private AudioClip crashSound;

    [SerializeField]
    private AudioClip enemySmashSound;

    [SerializeField]
    private AudioClip wallSmashSound;

    [SerializeField]
    private Image musicToggleImage;

    [SerializeField]
    private Image soundToggleImage;

    [SerializeField]
    private Sprite toggleOnSprite;

    [SerializeField]
    private Sprite toggleOffSprite;

    public bool soundOn = true;
    public bool musicOn = true;

    [HideInInspector]
    public Vector3 lastDeathPosition;

    [HideInInspector]
    public GameObject lastHitObstacle;

    [HideInInspector]
    public bool canReboot = false;

    void Awake()
    {
        instance = this;
        soundOn = PlayerPrefs.GetInt("soundOn", 1) == 1;
        musicOn = PlayerPrefs.GetInt("musicOn", 1) == 1;
    }

    void Start()
    {
        // soundDropdown.value = soundOn ? 0 : 1;
        // musicDropdown.value = musicOn ? 0 : 1;

        // soundDropdown.onValueChanged.AddListener(OnSoundChanged);
        // musicDropdown.onValueChanged.AddListener(OnMusicChanged);

        overallHighScore = PlayerPrefs.GetInt("HighScore", 0);

        overallHighScoreText.text = string.Format(
            LocalizationManager.Instance.GetText("BEST_SCORE"),
            overallHighScore
        );
        string gameLanguage = PlayerPrefs.GetString("GameLanguage");
        LanguageChange(gameLanguage);
        HandleMusic();

        if (restartFromGameOver)
        {
            restartFromGameOver = false;
            StartGame();
        }
        else
        {
            // restartFromMainMenu = false;
            SetState(GameState.Start);
        }
        // SetState(GameState.Update);

        UpdateToggleUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Running)
                PauseGame();
            else if (currentState == GameState.Paused)
                ResumeGame();
        }

        if (currentState != GameState.Running || player == null)
        {
            worldSpeed = 0f;
            return;
        }

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        worldSpeed =
            (playerBlocked || rb.linearVelocity.x <= 0.01f)
                ? 0f
                : rb.linearVelocity.x * speedMultiplier;

        score = (player.transform.position.x - playerStartX) * scoreRate + extraScore;

        int finalScore = Mathf.CeilToInt(score);

        GameScoreText.text = string.Format(
            LocalizationManager.Instance.GetText("SCORE_2"),
            finalScore
        );
    }

    public void StartGame()
    {
        AnalyticsLogger.LogGameStart();
        score = 0;
        extraScore = 0;
        playerStartX = player.transform.position.x;
        SetState(GameState.Running);
    }

    public void GameOver(GameObject hitObject = null)
    {
        if (currentState != GameState.Running)
            return;
        SetPaused(false);
        lastDeathPosition = player.transform.position;
        lastHitObstacle = hitObject;
        canReboot = true;

        int finalScore = Mathf.CeilToInt(score);

        GameOverScoreText.text = string.Format(
            LocalizationManager.Instance.GetText("SCORE_2"),
            finalScore
        );

        if (finalScore > overallHighScore)
        {
            overallHighScore = finalScore;
            PlayerPrefs.SetInt("HighScore", overallHighScore);
            PlayerPrefs.Save();
        }
        AnalyticsLogger.LogGameOver(finalScore, overallHighScore);

        overallHighScoreText.text = "HIGH SCORE : " + overallHighScore;
        SetState(GameState.GameOver);
    }

    public void SetState(GameState newState)
    {
        currentState = newState;

        gameStartScreen.SetActive(newState == GameState.Start);
        gameOverScreen.SetActive(newState == GameState.GameOver);
        inGameScreen.SetActive(newState == GameState.Running);
        pauseGameScreen.SetActive(newState == GameState.Paused);
        gameSettingScreen.SetActive(newState == GameState.Setting);
        LanguageScreen.SetActive(newState == GameState.Language);
        UpdateScreen.SetActive(newState == GameState.Update);
        if (player != null)
        {
            player.SetActive(newState == GameState.Running || newState == GameState.Paused);
        }
        SpawnManagerObject.SetActive(newState == GameState.Running);
        if (Prefeb_1 != null)
            Prefeb_1.SetActive(newState == GameState.Running);
        InGame_Scoretext.gameObject.SetActive(newState == GameState.Running);
    }

    //  public void SetState(GameState newState)
    // {
    //     if (currentState == newState)
    //         return;

    //     currentState = newState;

    //     void SafeSetActive(GameObject obj, bool condition)
    //     {
    //         if (obj != null && obj.activeSelf != condition)
    //         {
    //             obj.SetActive(condition);
    //         }
    //     }

    //     SafeSetActive(gameStartScreen, newState == GameState.Start);
    //     SafeSetActive(gameOverScreen, newState == GameState.GameOver);
    //     SafeSetActive(inGameScreen, newState == GameState.Running);
    //     SafeSetActive(pauseGameScreen, newState == GameState.Paused);
    //     SafeSetActive(gameSettingScreen, newState == GameState.Setting);
    //     SafeSetActive(LanguageScreen, newState == GameState.Language);
    //     SafeSetActive(UpdateScreen, newState == GameState.Update);

    //     SafeSetActive(player, newState == GameState.Running || newState == GameState.Paused);

    //     SafeSetActive(SpawnManagerObject, newState == GameState.Running);
    //     SafeSetActive(Prefeb_1, newState == GameState.Running);

    //     if (InGame_Scoretext != null && InGame_Scoretext.gameObject != null)
    //     {
    //         SafeSetActive(InGame_Scoretext.gameObject, newState == GameState.Running);
    //     }
    // }

    public void AddScore(float amount)
    {
        score += amount;
    }

    public void AddExtraScore(float amount)
    {
        extraScore += amount;
    }

    public void ShowSmashText()
    {
        smashCombo++;

        if (comboResetCoroutine != null)
            StopCoroutine(comboResetCoroutine);

        comboResetCoroutine = StartCoroutine(ComboResetTimer());
        if (smashCombo > 0)
        {
            smashText.text = string.Format(
                LocalizationManager.Instance.GetText("SMASH_2"),
                smashCombo
            );
        }
        else
        {
            smashText.text = LocalizationManager.Instance.GetText("SMASH");
        }
        StartCoroutine(SmashTextRoutine());
    }

    IEnumerator ComboResetTimer()
    {
        yield return new WaitForSeconds(comboResetTime);
        smashCombo = 0;
    }

    IEnumerator SmashTextRoutine()
    {
        smashText.gameObject.SetActive(true);
        yield return new WaitForSeconds(smashTextDuration);
        smashText.gameObject.SetActive(false);
    }

    public void ShowExecutedText()
    {
        ExecutedCombo++;

        if (executedResetCoroutine != null)
            StopCoroutine(executedResetCoroutine);

        executedResetCoroutine = StartCoroutine(ComboResetTimer2());
        executedText.text = ExecutedCombo <= 1 ? "EXECUTED" : $"EXECUTED x{ExecutedCombo}";
        StartCoroutine(ExecutedTextRoutine());
    }

    IEnumerator ComboResetTimer2()
    {
        yield return new WaitForSeconds(comboResetTime);
        ExecutedCombo = 0;
    }

    IEnumerator ExecutedTextRoutine()
    {
        executedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(smashTextDuration);
        executedText.gameObject.SetActive(false);
    }

    public void FullEnergy()
    {
        currentEnergy = Mathf.Clamp(currentEnergy + maxEnergy / 4f, 0, maxEnergy);
    }

    void SetPaused(bool paused)
    {
        Time.timeScale = paused ? 0f : 1f;
        isPaused = paused;
    }

    public void RestartGame()
    {
        InterstitialAdManager.Instance.AdShown++;
        if (InterstitialAdManager.Instance.AdShown % 2 == 0)
        {
            InterstitialAdManager.Instance.ShowInterstitialIfReady();
        }
        SetPaused(false);
        restartFromGameOver = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Reboot()
    {
        if (!canReboot)
            return;
        int finalScore2 = Mathf.CeilToInt(score);
        AnalyticsLogger.LogReboot(finalScore2);
        RewardedAdManager.Instance.ShowRewarded(
            () =>
            {
                SetPaused(false);
                OnRebootSuccess();
            },
            () =>
            {
                Debug.Log("No Ad Available");
                Instantiate(NoAdGameObject, gameOverScreen.transform);
            }
        );
    }

    void OnRebootSuccess()
    {
        canReboot = false;

        lastDeathPosition.y = lastDeathPosition.y <= -30 ? 5.05f : lastDeathPosition.y;

        player.SetActive(true);
        Prefeb_1.SetActive(true);
        player.transform.position = lastDeathPosition;
        lastDeathPosition.y = lastDeathPosition.y - 7f;
        Prefeb_1.transform.position = lastDeathPosition;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        PlayerController pc = player.GetComponent<PlayerController>();
        pc.ResetPlayer();
        // rb.linearVelocity = Vector2.zero;

        currentEnergy = currentEnergy > 10 ? currentEnergy : 20;

        if (lastHitObstacle != null && lastHitObstacle.CompareTag("Pink_Square"))
        {
            lastHitObstacle.SetActive(false);
        }

        SetState(GameState.Running);
    }

    public void PauseGame()
    {
        if (currentState != GameState.Running)
            return;
        AnalyticsLogger.LogGamePaused();
        SetPaused(true);
        SetState(GameState.Paused);
    }

    public void ResumeGame()
    {
        if (!isPaused)
            return;
        AnalyticsLogger.LogGameResume();
        SetPaused(false);
        SetState(GameState.Running);
    }

    public void ToggleSound()
    {
        soundOn = !soundOn;

        PlayerPrefs.SetInt("soundOn", soundOn ? 1 : 0);
        PlayerPrefs.Save();

        UpdateToggleUI();
    }

    public void ToggleMusic()
    {
        musicOn = !musicOn;

        HandleMusic();

        PlayerPrefs.SetInt("musicOn", musicOn ? 1 : 0);
        PlayerPrefs.Save();

        UpdateToggleUI();
    }

    public void ToggleSettingMode()
    {
        SetState(currentState == GameState.Setting ? GameState.Start : GameState.Setting);
    }

    private void HandleMusic()
    {
        if (musicSource == null)
            return;

        if (musicOn)
        {
            if (!musicSource.isPlaying)
                musicSource.Play();
        }
        else
        {
            musicSource.Stop();
        }
    }

    public void GotoMainMenu()
    {
        // if (currentState == GameState.Start)
        //     return;
        InterstitialAdManager.Instance.AdShown++;
        if (InterstitialAdManager.Instance.AdShown % 2 == 0)
        {
            InterstitialAdManager.Instance.ShowInterstitialIfReady();
        }
        SetPaused(false);
        // SetState(GameState.Start);
        // restartFromMainMenu = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (!soundOn || clip == null || sfxSource == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    public void PlayDashSound()
    {
        PlaySFX(dashSound);
    }

    public void PlayCrashSound()
    {
        PlaySFX(crashSound);
    }

    public void PlayEnemySmashSound()
    {
        PlaySFX(enemySmashSound);
    }

    public void PlayWallSmashSound()
    {
        PlaySFX(wallSmashSound);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnSoundChanged(int value)
    {
        soundOn = value == 0;
        PlayerPrefs.SetInt("soundOn", soundOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OnMusicChanged(int value)
    {
        musicOn = value == 0;
        HandleMusic();
        PlayerPrefs.SetInt("musicOn", musicOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void GoBackToSetting()
    {
        if (currentState != GameState.Language)
            return;

        SetState(GameState.Setting);
    }

    public void GoToLanguageScreen()
    {
        if (currentState != GameState.Setting)
            return;

        SetState(GameState.Language);
    }

    public void ToggleUpdateScreen()
    {
        SetState(currentState == GameState.Update ? GameState.Start : GameState.Update);
    }

    void UpdateToggleUI()
    {
        if (musicToggleImage != null)
            musicToggleImage.sprite = musicOn ? toggleOnSprite : toggleOffSprite;

        if (soundToggleImage != null)
            soundToggleImage.sprite = soundOn ? toggleOnSprite : toggleOffSprite;
    }

    public void OpenPrivacyPolicy()
    {
        Application.OpenURL("https://www.thegamewise.com/privacy-policy/");
    }

    public void LanguageChange(string languageName)
    {
        englishCheck.SetActive(false);
        portugueseCheck.SetActive(false);
        spanishCheck.SetActive(false);
        frenchCheck.SetActive(false);

        switch (languageName)
        {
            case "English":
                englishCheck.SetActive(true);
                LocalizationManager.Instance.SetLanguage("en");
                break;

            case "Portuguese":
                portugueseCheck.SetActive(true);
                LocalizationManager.Instance.SetLanguage("pt-BR");
                break;

            case "Spanish":
                spanishCheck.SetActive(true);
                LocalizationManager.Instance.SetLanguage("sp");
                break;

            case "French":
                frenchCheck.SetActive(true);
                LocalizationManager.Instance.SetLanguage("fr");
                break;
            default:
                englishCheck.SetActive(true);
                LocalizationManager.Instance.SetLanguage("en");
                break;
        }

        overallHighScore = PlayerPrefs.GetInt("HighScore", 0);

        overallHighScoreText.text = string.Format(
            LocalizationManager.Instance.GetText("BEST_SCORE"),
            overallHighScore
        );
        gameLanguageText.text = languageName;
        PlayerPrefs.SetString("GameLanguage", languageName);
        PlayerPrefs.Save();
        SetState(GameState.Setting);
    }
}
