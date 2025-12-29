using UnityEngine;

public class YellowWallBox : MonoBehaviour
{
    private float breakVelocity = 12f;
    public GameObject breakFX;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;
        float impactSpeed = collision.relativeVelocity.magnitude;

        if (impactSpeed >= breakVelocity)
        {
            Break();
        }
    }

    private void Break()
    {
        if (breakFX != null)
        {
            Instantiate(breakFX, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
