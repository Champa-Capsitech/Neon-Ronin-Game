using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    private float baseSpeed = 8f;   // minimum speed
    //private float speedMultiplier = 1f;
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

        if (GameManager.instance.playerBlocked)
            return; //  STOP MOVING WORLD

        transform.Translate(Vector2.left * baseSpeed * Time.deltaTime, Space.World);

        if (transform.position.x < leftBound)
        {
            Destroy(gameObject);
        }
    }


}
