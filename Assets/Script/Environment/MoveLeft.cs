using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    void Update()
    {
        if (GameManager.instance.currentState != GameManager.GameState.Running)
            return;

        float worldSpeed = GameManager.instance.worldSpeed;

        if (worldSpeed <= 0f)
            return;

        // Move object left relative to player
        transform.Translate(Vector2.left * worldSpeed * Time.deltaTime, Space.World);

        DestroyIfOffCamera();
    }

    void DestroyIfOffCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        float leftEdge =
            cam.transform.position.x - cam.orthographicSize * cam.aspect;

        // Small buffer so object is fully off-screen
        if (transform.position.x < leftEdge - 2f)
        {
            Destroy(gameObject);
        }
    }
}

