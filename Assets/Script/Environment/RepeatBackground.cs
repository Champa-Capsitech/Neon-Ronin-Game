using UnityEngine;

public class RepeatBackground : MonoBehaviour
{
    float width;
    Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
        width = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        if (transform.position.x + width < cam.position.x)
        {
            transform.position += Vector3.right * width * 2f;
        }
    }
}
