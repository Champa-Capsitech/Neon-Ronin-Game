using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform player;
    public float fixedZ = -5f;
    public Vector3 offset;

    void LateUpdate()
    {
        if (!player)
            return;

        transform.position = new Vector3(
            player.position.x + offset.x,
            player.position.y + offset.y,
            fixedZ
        );
    }
}