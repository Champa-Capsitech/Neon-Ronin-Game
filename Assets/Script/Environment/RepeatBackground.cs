using UnityEngine;

public class RepeatBackground : MonoBehaviour
{
    private Vector2 startPos;

    public float divide = 2f;

    private float moveSpeed = 8f;

    private float repeatDistance;

    Rigidbody2D PlayerRB;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            PlayerRB = player.GetComponent<Rigidbody2D>();
            //moveSpeed = PlayerRB.linearVelocity.y;
            //moveSpeed = 8;
            //Debug.Log("speed" + moveSpeed);
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
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);

        if (transform.position.x <= startPos.x - repeatDistance)
        {
            transform.position = startPos;
        }
    }
}
