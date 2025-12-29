using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private float playerStartX; //player's starting pos at x = 0


    [SerializeField] GameObject player;

    public enum GameState
    {
        Start,
        Running,
        GameOver
    }

    public GameState currentState;


    [Header("UI Screens")]
    public GameObject gameStartScreen;
    public GameObject gameOverScreen;
    public TextMeshProUGUI GameScoreText;
    public TextMeshProUGUI GameOverScoreText;



    [Header("Score")]
    public float score;
    public float scoreRate = 10f;


    public static bool restartFromGameOver = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
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
        if (currentState == GameState.Running && player != null)
        {
            // Distance travelled relative to start
            float distanceTravelled = player.transform.position.x - playerStartX;

            score = distanceTravelled * scoreRate;

            GameScoreText.text = "SCORE : " + Mathf.CeilToInt(score);
        }
    }


    //STATE CONTROL

    public void StartGame()
    {
        score = 0;
        playerStartX = player.transform.position.x;  // <-- add this line
        Debug.Log("StartGame ");
        SetState(GameState.Running);
    }



    public void GameOver()
    {
        if (currentState != GameState.Running) return;

        Debug.Log("Game Over!!");
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

        Debug.Log("Current State: " + newState);

        gameStartScreen.SetActive(newState == GameState.Start);
        gameOverScreen.SetActive(newState == GameState.GameOver);

        if (newState == GameState.Start)
        {
            player.SetActive(false);
            Debug.Log("GameState.Start");
        }
        else if (newState == GameState.Running)
        {
            player.SetActive(true);
            Debug.Log("GameState.Running");
        }
    }



    //SCORE

    public void AddScore(float amount)
    {
        score += amount;

    }
}

