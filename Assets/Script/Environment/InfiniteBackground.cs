using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    public Transform cam;
    public float scrollMultiplier = 1f;

    Material mat;

    void Start()
    {
        mat = GetComponent<SpriteRenderer>().material;
    }

    void Update()
    {
        Vector3 camPos = cam.position;

        Vector2 offset = new Vector2(camPos.x * scrollMultiplier, camPos.y * scrollMultiplier);

        mat.mainTextureOffset = offset;
    }
}
