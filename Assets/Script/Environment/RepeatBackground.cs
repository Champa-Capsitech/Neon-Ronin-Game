using UnityEngine;

public class RepeatBackground : MonoBehaviour
{
    float width;
    float height;
    Transform cam;

    void Start()
    {
        cam = Camera.main.transform;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        width = sr.bounds.size.x;
        height = sr.bounds.size.y;
    }

    void Update()
    {
        if (cam.position.x > transform.position.x + width)
        {
            transform.position += Vector3.right * width * 2f;
        }
        else if (cam.position.x < transform.position.x - width)
        {
            transform.position += Vector3.left * width * 2f;
        }

        if (cam.position.y > transform.position.y + height)
        {
            transform.position += Vector3.up * height * 2f;
        }
        else if (cam.position.y < transform.position.y - height)
        {
            transform.position += Vector3.down * height * 2f;
        }
    }
}