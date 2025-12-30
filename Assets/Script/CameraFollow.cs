using UnityEngine;

public class CameraFollowY : MonoBehaviour
{
    public Transform player;

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 pos = transform.position;
        pos.y = player.position.y; // directly follow player
        transform.position = pos;
    }
}
