

using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    private float baseSpeed = 8f;   // minimum speed
    private float speedMultiplier = 1f;
    private float leftBound = -25f;

    Rigidbody2D PlayerRB;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            PlayerRB = player.GetComponent<Rigidbody2D>();
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
        //if (PlayerRB == null) return;
        //float playerSpeedX = PlayerRB.linearVelocity.x;

        //float effectiveSpeed = Mathf.Max(0f, playerSpeedX);

        //transform.Translate(Vector2.left * effectiveSpeed * Time.deltaTime);
        if (transform.position.x < leftBound)
        {
            Destroy(gameObject);
        }
    }
}
