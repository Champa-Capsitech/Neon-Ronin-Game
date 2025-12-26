using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    private float baseSpeed = 8f;   // minimum speed
    private float speedMultiplier = 1f;
    private float leftBound = -25f;

    Rigidbody2D playerRB;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerRB = player.GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        if (GameManager.instance.currentState != GameManager.GameState.Running)
            return;

        float playerSpeed = baseSpeed;

        //if (playerRB != null)
        //{
        //    playerSpeed += Mathf.Abs(playerRB.linearVelocity.y) * speedMultiplier;
        //}

        transform.Translate(Vector2.left * playerSpeed * Time.deltaTime, Space.World);

        if (transform.position.x < leftBound && !CompareTag("Background"))
        {
            Destroy(gameObject);
        }
    }
}
