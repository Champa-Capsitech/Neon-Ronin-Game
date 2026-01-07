using UnityEngine;

public class ShockwaveEffect : MonoBehaviour
{
    public float expandSpeed = 3f;
    public float lifeTime = 0.5f;

    SpriteRenderer sr;
    Color startColor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        startColor = sr.color;

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Expand the ring
        transform.localScale += Vector3.one * expandSpeed * Time.deltaTime;

        // Fade out over time
        float fadeStep = Time.deltaTime / lifeTime;
        sr.color = new Color(
            startColor.r,
            startColor.g,
            startColor.b,
            sr.color.a - fadeStep
        );
    }
}
