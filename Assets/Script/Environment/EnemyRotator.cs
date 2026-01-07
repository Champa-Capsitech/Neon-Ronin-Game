using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    private float breakVelocity = 5f;
    public int scoreReward = 500;
    public float rotationSpeed = 90f;

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
            Destroy(gameObject);          
        }
        else
        {
            GameManager.instance.GameOver();
            Destroy(other.gameObject);    
        }
    }
}
