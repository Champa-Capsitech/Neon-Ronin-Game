using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    [Header("Safe Area (world units)")]
    public float safeZoneHalfHeight = 2.5f;
    public float safeZoneHalfWidth = 3.5f;

    [Header("Camera Shake")]
    public float shakeStrength = 2f;
    public float shakeDuration = 0.15f;

    Vector3 basePosition;
    float shakeTimer;

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 camPos = transform.position;

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

        basePosition = camPos;

        if (shakeTimer > 0f)
        {
            camPos += (Vector3)Random.insideUnitCircle * shakeStrength;
            shakeTimer -= Time.deltaTime;
        }

        transform.position = camPos;
    }

    public void Shake()
    {
        shakeTimer = shakeDuration;
    }
}