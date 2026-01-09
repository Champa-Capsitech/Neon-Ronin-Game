using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float breakVelocity = 10f;
    private int scoreReward = 500;
    private float rotationSpeed = 90f;


    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
        if (playerRb == null) return;

        float playerSpeed = playerRb.linearVelocity.magnitude;

        if (playerSpeed >= breakVelocity)
        {
            GameManager.instance.AddExtraScore(scoreReward);
            GameManager.instance.ShowExecutedText();
            GameManager.instance.FullEnergy();
            Destroy(gameObject);
        }
        else
        {
            GameManager.instance.GameOver();
            Destroy(other.gameObject);
        }
    }
}
