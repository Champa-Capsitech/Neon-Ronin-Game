using UnityEngine;

public class YellowWallBox : MonoBehaviour
{
    [Header("Break Settings")]
    public float breakVelocity = 12f;

    [Header("FX")]
    public GameObject breakFX; // Yellow particle prefab

    private bool isBroken = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBroken) return;

        //Only Player can interact
         if (!collision.collider.CompareTag("Player"))
            return;

        PlayerController player = collision.collider.GetComponent<PlayerController>();
        if (player == null)
            return;

        float impactSpeed = collision.relativeVelocity.magnitude;

        // Break ONLY if dashing AND fast enough
        if (player.isDragging && impactSpeed >= breakVelocity)
        {
            Break(player);
        }
    }

    private void Break(PlayerController player)
    {
        isBroken = true;

        // Reset dash / energy
        //player.ResetDash();

        // Spawn yellow particle effect
        if (breakFX != null)
        {
            Instantiate(breakFX, transform.position, Quaternion.identity);
        }

        //Destroy(gameObject);
    }
}
