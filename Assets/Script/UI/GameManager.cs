using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private float playerStartX; //player's starting pos at x = 0
    public bool playerBlocked;



    [SerializeField] GameObject player;

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
            float distanceTravelled = player.transform.position.x - playerStartX;

            score = distanceTravelled * scoreRate + extraScore;

            GameScoreText.text = "SCORE : " + Mathf.CeilToInt(score);
        }
    }

    public void StartGame()
    {
        score = 0;
        playerStartX = player.transform.position.x;
        // Debug.Log("StartGame ");
        SetState(GameState.Running);
    }



    public void GameOver()
    {
        // Debug.Log("Game Over!!");
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

        // Debug.Log("Current State: " + newState);

        gameStartScreen.SetActive(newState == GameState.Start);
        gameOverScreen.SetActive(newState == GameState.GameOver);

        if (newState == GameState.Start)
        {
            player.SetActive(false);
            // Debug.Log("GameState.Start");
        }
        else if (newState == GameState.Running)
        {
            player.SetActive(true);
            // Debug.Log("GameState.Running");
        }
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

        UpdateSmashText();
        StartCoroutine(SmashTextRoutine());
    }

    private IEnumerator ComboResetTimer()
    {
        yield return new WaitForSeconds(comboResetTime);
        smashCombo = 0;
    }

    private void UpdateSmashText()
    {
        if (smashCombo <= 1)
            smashText.text = "SMASH";
        else
            smashText.text = $"SMASH x{smashCombo}";
    }


    private IEnumerator SmashTextRoutine()
    {
        smashText.gameObject.SetActive(true);

        RectTransform rect = smashText.rectTransform;

        rect.localScale = Vector3.zero;

        float popTime = 0.15f;
        float timer = 0f;

        while (timer < popTime)
        {
            timer += Time.deltaTime;
            float t = timer / popTime;

            rect.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 1.2f, t);
            yield return null;
        }

        rect.localScale = Vector3.one;

        yield return new WaitForSeconds(smashTextDuration);

        smashText.gameObject.SetActive(false);
    }

}

