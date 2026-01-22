using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    [Range(0f, 1f)]
    public float safeZonePercent = 0.4f; 

    private float cameraTopLimitY = 3f;

    public float shakeStrength = 2f;
    public float shakeDuration = 0.15f;

    private float followStartOffsetY = 0f;

    float shakeTimer;
    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 cameraPosition = transform.position;

        float cameraHalfHeight = cam.orthographicSize;
        float cameraHalfWidth = cam.orthographicSize * cam.aspect;

        float safeZoneWidth = cameraHalfWidth * 2f * safeZonePercent;

        float cameraLeftX = cameraPosition.x - cameraHalfWidth;
        float safeZoneRightX = cameraLeftX + safeZoneWidth;

        if (player.position.x > safeZoneRightX)
        {
            float moveAmount = player.position.x - safeZoneRightX;
            cameraPosition.x += moveAmount;
        }

        float followLineY = cameraPosition.y + followStartOffsetY;

        if (player.position.y < followLineY)
        {
            float moveAmount = player.position.y - followLineY;
            cameraPosition.y += moveAmount;
        }

        if (player.position.y > followLineY)
        {
            float moveAmount = player.position.y - followLineY;
            cameraPosition.y += moveAmount;
        }

        float maxCameraY = cameraTopLimitY - cameraHalfHeight;
        if (cameraPosition.y > maxCameraY)
        {
            cameraPosition.y = maxCameraY;
        }

        if (shakeTimer > 0f)
        {
            Vector2 randomShake = Random.insideUnitCircle * shakeStrength;
            cameraPosition.x += randomShake.x;
            cameraPosition.y += randomShake.y;

            shakeTimer -= Time.deltaTime;
        }

        transform.position = cameraPosition;
    }

    public void Shake()
    {
        shakeTimer = shakeDuration;
    }
}
