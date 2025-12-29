using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 5f;
    public Vector3 offset;
    void LateUpdate()
    {
        if (player == null)
            return;

        if (player.position.y < transform.position.y)
        {
            Vector3 targetPosition = new Vector3(
                transform.position.x,          
                player.position.y + offset.y,   
                transform.position.z            
            );

            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                followSpeed * Time.deltaTime
            );
        }
    }
}
