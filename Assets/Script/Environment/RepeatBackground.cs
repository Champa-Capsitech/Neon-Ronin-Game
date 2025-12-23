using UnityEngine;

public class RepeatBackground : MonoBehaviour
{
    private Vector2 startPos;

    public float divide = 2f;

    public float moveSpeed = 5f;

    private float repeatDistance;

    void Start()
    {
        startPos = transform.position;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float spriteWidth = sr.bounds.size.x;

        repeatDistance = spriteWidth / divide;
    }

    void Update()
    {
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);

        if (transform.position.x <= startPos.x - repeatDistance)
        {
            transform.position = startPos;
        }
    }
}
