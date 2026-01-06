using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    [Header("Safe Area (world units)")]
    public float safeZoneHalfHeight = 2.5f; 
    public float safeZoneHalfWidth = 3.5f;  

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 camPos = transform.position;

        // Difference between player and camera
        float deltaY = player.position.y - camPos.y;
        float deltaX = player.position.x - camPos.x;

        if (Mathf.Abs(deltaY) > safeZoneHalfHeight)
        {
            camPos.y += deltaY - Mathf.Sign(deltaY) * safeZoneHalfHeight;
        }

        if (Mathf.Abs(deltaX) > safeZoneHalfWidth)
        {
            camPos.x += deltaX - Mathf.Sign(deltaX) * safeZoneHalfWidth;
        }

        transform.position = camPos;
    }
}
