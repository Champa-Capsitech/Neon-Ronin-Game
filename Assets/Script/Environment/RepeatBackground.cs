using UnityEngine;

public class RepeatBackground : MonoBehaviour
{
    private Vector2 startPos;
    private float divide = 4f;
    private float repeatDistance;
    Rigidbody2D PlayerRB;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            PlayerRB = player.GetComponent<Rigidbody2D>();
        }
    }
    void Start()
    {
        startPos = transform.position;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float spriteWidth = sr.bounds.size.x;

        repeatDistance = spriteWidth / divide;
    }

    void Update()
    {
        if (GameManager.instance.currentState != GameManager.GameState.Running)
            return;

        if (PlayerRB == null) return;
        float playerSpeedX = PlayerRB.linearVelocity.x;

        float effectiveSpeed = Mathf.Max(0f, playerSpeedX);

        transform.Translate(Vector2.left * effectiveSpeed * Time.deltaTime);

        if (transform.position.x <= startPos.x - repeatDistance)
        {
            transform.position = startPos;
        }
    }
}
