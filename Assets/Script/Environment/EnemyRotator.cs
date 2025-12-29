using UnityEngine;

public class EnemyRotator : MonoBehaviour
{
    public float rotationSpeed = 90f;

    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        //Destroy(collision.gameObject);
        Destroy(gameObject);
        GameManager.instance.GameOver();
    }
}
