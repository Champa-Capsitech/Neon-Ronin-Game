using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    GameObject player;
    public TextMeshProUGUI InGame_Scoretext;
    private float playerStartX;

    public float worldSpeed;
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
    }

    public TMP_Dropdown soundDropdown;
    public TMP_Dropdown musicDropdown;
    public GameObject gameSettingScreen;
    public GameState currentState;
    public GameObject gameStartScreen;
    public GameObject gameOverScreen;
    public GameObject pauseGameScreen;
    public GameObject inGameScreen;
    public TextMeshProUGUI GameScoreText;
    public TextMeshProUGUI GameOverScoreText;
    public GameObject LanguageScreen;
    public string currentLanguage;


    public GameObject englishCheck;
    public GameObject portugueseCheck;
    public GameObject russianCheck;
    public GameObject frenchCheck;
    public float score;
    public float extraScore = 0;
    public float scoreRate = 10f;

    public static bool restartFromGameOver = false;

    public TextMeshProUGUI smashText;
    public TextMeshProUGUI executedText;
    private float smashTextDuration = 0.5f;
    private int smashCombo = 0;
    private int ExecutedCombo = 0;
    private float comboResetTime = 0.5f;
    private Coroutine comboResetCoroutine;
    private Coroutine executedResetCoroutine;

    public float maxEnergy = 100f;
    public float currentEnergy;

    public int overallHighScore;
    public TextMeshProUGUI overallHighScoreText;
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

    
    
    [SerializeField] private Image musicToggleImage;
    [SerializeField] private Image soundToggleImage;

    [SerializeField] private Sprite toggleOnSprite;
    [SerializeField] private Sprite toggleOffSprite;

    
    
    public bool soundOn = true;
    public bool musicOn = true;

    void Awake()
    {
        instance = this;

        soundOn = PlayerPrefs.GetInt("soundOn", 1) == 1;
        musicOn = PlayerPrefs.GetInt("musicOn", 1) == 1;
    }

    void Start()
    {
        soundDropdown.value = soundOn ? 0 : 1;
        musicDropdown.value = musicOn ? 0 : 1;

        soundDropdown.onValueChanged.AddListener(OnSoundChanged);
        musicDropdown.onValueChanged.AddListener(OnMusicChanged);

        overallHighScore = PlayerPrefs.GetInt("HighScore", 0);
        overallHighScoreText.text = "BEST SCORE : " + overallHighScore;
        string gameLanguage = PlayerPrefs.GetString("GameLanguage", "");
        ChangeLanguage(gameLanguage);
        HandleMusic();

        if (restartFromGameOver)
        {
            restartFromGameOver = false;
            StartGame();
        }
        else
        {
            SetState(GameState.Start);
        }

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
        GameScoreText.text = "SCORE : " + Mathf.CeilToInt(score);
    }

    public void StartGame()
    {
        score = 0;
        extraScore = 0;
        playerStartX = player.transform.position.x;
        SetState(GameState.Running);
    }

    public void GameOver()
    {
        if (currentState != GameState.Running)
            return;
        SetPaused(false);
        //InterstitialAdManager.Instance.ShowInterstitialIfReady();

        int finalScore = Mathf.CeilToInt(score);
        GameOverScoreText.text = "SCORE : " + finalScore;

        if (finalScore > overallHighScore)
        {
            overallHighScore = finalScore;
            PlayerPrefs.SetInt("HighScore", overallHighScore);
            PlayerPrefs.Save();
        }

        overallHighScoreText.text = "HIGH SCORE : " + overallHighScore;
        SetState(GameState.GameOver);
    }

    void SetState(GameState newState)
    {
        currentState = newState;

        gameStartScreen.SetActive(newState == GameState.Start);
        gameOverScreen.SetActive(newState == GameState.GameOver);
        inGameScreen.SetActive(newState == GameState.Running);
        pauseGameScreen.SetActive(newState == GameState.Paused);
        gameSettingScreen.SetActive(newState == GameState.Setting);
        LanguageScreen.SetActive(newState == GameState.Language);

        if (newState == GameState.Start)
        {
            player.SetActive(false);
        }
        else if (newState == GameState.Running)
        {
            player.SetActive(true);
        }
        InGame_Scoretext.gameObject.SetActive(newState == GameState.Running);
    }

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
        smashText.text = smashCombo <= 1 ? "SMASH" : $"SMASH x{smashCombo}";
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
        //RewardedAdManager.Instance.ShowRewarded(() =>
        //{
            SetPaused(false);
            restartFromGameOver = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //});
    }

    // public void RestartGame()
    // {
    //     RewardedAdManager.Instance.ShowRewarded();
    //     executedResetCoroutine = StartCoroutine(RestartGameRoutine());
    //     SetPaused(false);
    //     restartFromGameOver = true;
    //     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    // }

    // IEnumerator RestartGameRoutine()
    // {
    //     smashText.gameObject.SetActive(true);
    //     yield return new WaitForSeconds(5);
    //     smashText.gameObject.SetActive(false);
    // }

    public void PauseGame()
    {
        if (currentState != GameState.Running)
            return;

        SetPaused(true);
        SetState(GameState.Paused);
    }

    public void ResumeGame()
    {
        if (!isPaused)
            return;

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
        if (currentState == GameState.Start)
            return;

        //InterstitialAdManager.Instance.ShowInterstitialIfReady();
        SetPaused(false);
        // SetState(GameState.Start);
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

    void UpdateToggleUI()
    {
        if (musicToggleImage != null)
            musicToggleImage.sprite = musicOn ? toggleOnSprite : toggleOffSprite;

        if (soundToggleImage != null)
            soundToggleImage.sprite = soundOn ? toggleOnSprite : toggleOffSprite;
    }

    public void ChangeLanguage(string languageName)
    {
        englishCheck.SetActive(false);
        portugueseCheck.SetActive(false);
        russianCheck.SetActive(false);
        frenchCheck.SetActive(false);

        switch (languageName)
        {
            case "English":
                englishCheck.SetActive(true);
                break;

            case "Portuguese":
                portugueseCheck.SetActive(true);
                break;

            case "Russian":
                russianCheck.SetActive(true);
                break;

            case "French":
                frenchCheck.SetActive(true);
                break;
        }
        PlayerPrefs.SetString("GameLanguage", languageName);
        PlayerPrefs.Save();

        Debug.Log("Language Changed To: " + languageName);
    }
}
