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

    //PlayerController player = collision.GetComponent<PlayerController>();

    //if (player == null) return;

    //if (player.IsDashing)
    //{
    //    player.ResetDash();
    //    Destroy(gameObject);
    //}
    //else
    //{
    //    player.Die();
    //}
    }
}
