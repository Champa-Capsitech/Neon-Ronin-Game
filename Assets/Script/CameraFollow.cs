using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Player reference
    public Transform player;

    // How fast camera follows
    public float followSpeed = 5f;

    // Offset from player
    public Vector3 offset;

    void LateUpdate()
    {
        if (player == null)
            return;

        // Only follow when player goes DOWN
        if (player.position.y < transform.position.y)
        {
            Vector3 targetPosition = new Vector3(
                transform.position.x,           // keep X fixed
                player.position.y + offset.y,   // follow Y
                transform.position.z            // keep Z fixed
            );

            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                followSpeed * Time.deltaTime
            );
        }
    }
}
