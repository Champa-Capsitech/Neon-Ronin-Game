using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(TrailRenderer))]
[RequireComponent(typeof(EnergySystem))]
public class PlayerController : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashForce = 15f;

    [Header("Physics")]
    public float gravityScale = 0.65f;
    public float airDrag = 2f;

    [Header("Score")]
    public float distanceScore;

    

    Rigidbody2D rb;
    TrailRenderer trail;
    EnergySystem energy;

    Vector2 pointerStart;
    Vector2 pointerCurrent;
    bool isDragging;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();
        energy = GetComponent<EnergySystem>();

        rb.gravityScale = gravityScale;
        rb.linearDamping = airDrag;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        trail.emitting = false;
    }

    void Update()
    {
        HandlePointerInput();
        UpdateScore();
    }

    void HandlePointerInput()
    {
        // POINTER DOWN
        if (Input.GetMouseButtonDown(0))
        {
            pointerStart = ScreenToWorld(Input.mousePosition);
            isDragging = true;
        }

        // POINTER DRAG
        if (Input.GetMouseButton(0) && isDragging)
        {
            pointerCurrent = ScreenToWorld(Input.mousePosition);
        }

        // POINTER UP → DASH
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            TryDash();
            isDragging = false;
        }
    }

    void TryDash()
    {
        if (!energy.CanDash())
            return;

        Vector2 dragDirection = pointerCurrent - pointerStart;

        // ONLY Y AXIS MOVEMENT
        float yDirection = Mathf.Sign(dragDirection.y);
        if (yDirection == 0)
            return;

        energy.ConsumeDashEnergy(); // 🔥 ENERGY DECREASES HERE

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(Vector2.up * yDirection * dashForce, ForceMode2D.Impulse);

        trail.emitting = true;
        Invoke(nameof(StopTrail), 0.15f);
    }

    void StopTrail()
    {
        trail.emitting = false;
    }

    void UpdateScore()
    {
        distanceScore += Time.deltaTime * 10f;
    }

    Vector2 ScreenToWorld(Vector2 screenPos)
    {
        Vector3 world = Camera.main.ScreenToWorldPoint(screenPos);
        return new Vector2(world.x, world.y);
    }
}

