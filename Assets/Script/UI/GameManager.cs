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
    public bool soundOn = true;
    public bool musicOn = true;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        soundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
        musicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;

        AudioListener.volume = soundOn ? 1f : 0f;
    }

    void Start()
    {
        soundDropdown.value = soundOn ? 0 : 1;
        musicDropdown.value = musicOn ? 0 : 1;

        soundDropdown.onValueChanged.AddListener(OnSoundChanged);
        musicDropdown.onValueChanged.AddListener(OnMusicChanged);

        overallHighScore = PlayerPrefs.GetInt("HighScore", 0);
        if (restartFromGameOver)
        {
            restartFromGameOver = false;
            StartGame();
        }
        else
        {
            SetState(GameState.Start);
        }
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

        if (playerBlocked || rb.linearVelocity.x <= 0.01f)
            worldSpeed = 0f;
        else
            worldSpeed = rb.linearVelocity.x * speedMultiplier;

        float distanceTravelled = player.transform.position.x - playerStartX;
        score = distanceTravelled * scoreRate + extraScore;
        GameScoreText.text = "SCORE : " + Mathf.CeilToInt(score);

        // Debug.Log($"Sound: {soundOn}");
        // Debug.Log($"Sound: {soundOn}, Music: {musicOn}");
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
        Time.timeScale = 1f;
        isPaused = false;
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

    // Smash UI
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

    // Executed UI
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
        currentEnergy = maxEnergy;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        restartFromGameOver = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseGame()
    {
        if (currentState != GameState.Running)
            return;

        Time.timeScale = 0f;
        isPaused = true;
        SetState(GameState.Paused);
    }

    public void ResumeGame()
    {
        if (!isPaused)
            return;

        Time.timeScale = 1f;
        isPaused = false;
        SetState(GameState.Running);
    }

    public void ToggleSound()
    {
        soundOn = !soundOn;

        PlayerPrefs.SetInt("SoundOn", soundOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleSettingMode()
    {
        if (currentState == GameState.Setting)
            SetState(GameState.Start);
        else
            SetState(GameState.Setting);
    }

    public void GotoMainMenu()
    {
        if (currentState == GameState.Start)
            return;
        Time.timeScale = 1f;
        isPaused = false;
        SetState(GameState.Start);
        // restartFromGameOver = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

        AudioListener.volume = soundOn ? 1f : 0f;

        PlayerPrefs.SetInt("SoundOn", soundOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OnMusicChanged(int value)
    {
        musicOn = value == 0;

        // MusicManager.Instance.SetMusic(musicOn);

        PlayerPrefs.SetInt("MusicOn", musicOn ? 1 : 0);
        PlayerPrefs.Save();
    }
}
