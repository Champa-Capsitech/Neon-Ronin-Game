using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform player;
    public float fixedZ = -5f;

    void LateUpdate()
    {
        if (!player)
            return;

        transform.position = new Vector3(
            player.position.x,
            player.position.y,
            fixedZ
        );
    }
}