using UnityEngine;

public class MoveLeft : MonoBehaviour
{

    private float speed=8;
    private float leftBound = -13;
    Rigidbody2D PlayerRB;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            PlayerRB = player.GetComponent<Rigidbody2D>();
            //speed = PlayerRB.linearVelocity.y;
            //speed = 8;
        }
    }


    //background move left
    void Update()
    {
        if (GameManager.instance.currentState == GameManager.GameState.Running)
        {
            //Debug.Log("speed" + speed);
            transform.Translate(Vector2.left * speed * Time.deltaTime, Space.World);
        }
        if (transform.position.x < leftBound && !gameObject.CompareTag("Background"))
        {
            Destroy(gameObject);
        }

    }
}

