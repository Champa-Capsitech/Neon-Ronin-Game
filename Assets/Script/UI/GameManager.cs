using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

 
    [SerializeField] GameObject player;
    public TextMeshProUGUI InGame_Scoretext;
    private float playerStartX;


    public float worldSpeed;
    public float speedMultiplier = 1f;
    public bool playerBlocked;

    public enum GameState
    {
        Start,
        Running,
        GameOver
    }

    public GameState currentState;

  
    public GameObject gameStartScreen;
    public GameObject gameOverScreen;
    public TextMeshProUGUI GameScoreText;
    public TextMeshProUGUI GameOverScoreText;

   
    public float score;
    public float extraScore = 0;
    public float scoreRate = 10f;

    public static bool restartFromGameOver = false;

 
    public TextMeshProUGUI smashText;
    private float smashTextDuration = 0.5f;
    private int smashCombo = 0;
    private float comboResetTime = 0.5f;
    private Coroutine comboResetCoroutine;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
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
        if (currentState != GameState.Running) return;

        GameOverScoreText.text = "SCORE : " + Mathf.CeilToInt(score);
        SetState(GameState.GameOver);
    }

    public void RestartGame()
    {
        restartFromGameOver = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void SetState(GameState newState)
    {
        currentState = newState;
        gameStartScreen.SetActive(newState == GameState.Start);
        gameOverScreen.SetActive(newState == GameState.GameOver);

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
}

