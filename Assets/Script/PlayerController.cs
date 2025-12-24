//using UnityEngine;

////[RequireComponent(typeof(Rigidbody2D))]
////[RequireComponent(typeof(DashInput))]
////[RequireComponent(typeof(EnergySystem))]
////[RequireComponent(typeof(TrailRenderer))]
//public class PlayerController : MonoBehaviour
//{
//    [Header("Dash Settings")]
//    public float dashForce = 18f;
//    public float dashDuration = 0.15f;

//    [Header("Fall Death Settings")]
//    [Range(0f, 1f)]
//    public float lowEnergyThreshold = 0.25f; // 25%

//    Rigidbody2D rb;
//    DashInput dashInput;
//    EnergySystem energy;
//    TrailRenderer trail;

//    public bool isDashing { get; private set; }

//    void Awake()
//    {
//        rb = GetComponent<Rigidbody2D>();
//        dashInput = GetComponent<DashInput>();
//        energy = GetComponent<EnergySystem>();
//        trail = GetComponent<TrailRenderer>();

//        trail.emitting = false;
//        isDashing = false;
//    }

//    void Update()
//    {
//        // Dash happens ONLY on pointer release
//        if (Input.GetMouseButtonUp(0))
//        {
//            TryDash();
//        }
//    }

//    void FixedUpdate()
//    {
//        HandleFallDeath();
//    }

//    void TryDash()
//    {
//        // Block dash if energy is insufficient
//        if (!energy.CanDash())
//            return;

//        Vector2 dir = dashInput.direction;

//        // Safety check
//        if (dir == Vector2.zero)
//            return;

//        // Consume energy
//        energy.ConsumeDashEnergy();

//        // Reset current movement
//        rb.linearVelocity = Vector2.zero;

//        // Apply dash impulse
//        rb.AddForce(dir * dashForce, ForceMode2D.Impulse);

//        // Dash state
//        isDashing = true;
//        trail.emitting = true;

//        Invoke(nameof(EndDash), dashDuration);
//    }

//    void EndDash()
//    {
//        isDashing = false;
//        trail.emitting = false;
//    }

//    void HandleFallDeath()
//    {
//        // Player must be falling downward
//        if (rb.linearVelocity.y >= -0.1f)
//            return;

//        // If energy is below threshold → die
//        if (energy.currentEnergy <= energy.maxEnergy * lowEnergyThreshold)
//        {
//            Die();
//        }
//    }

//    public void Die()
//    {
//        // Player-side only
//        // GameManager / UI will react to this
//        gameObject.SetActive(false);
//    }
//}







//using UnityEngine;

//[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(DashInput))]
//[RequireComponent(typeof(EnergySystem))]
//[RequireComponent(typeof(TrailRenderer))]
//public class PlayerController : MonoBehaviour
//{
//    [Header("Dash Settings")]
//    public float dashForce = 18f;
//    public float dashDuration = 0.15f;

//    [Header("Energy Death")]
//    [Range(0f, 1f)]
//    public float lowEnergyThreshold = 0.25f;

//    [Header("Movement Bounds")]
//    public float Xmin = -10f;
//    public float Xmax = 10f;
//    public float Ymin = -4f;
//    public float Ymax = 4f;

//    Rigidbody2D rb;
//    DashInput dashInput;
//    EnergySystem energy;
//    TrailRenderer trail;

//    public bool isDashing { get; private set; }

//    void Awake()
//    {
//        rb = GetComponent<Rigidbody2D>();
//        dashInput = GetComponent<DashInput>();
//        energy = GetComponent<EnergySystem>();
//        trail = GetComponent<TrailRenderer>();

//        rb.gravityScale = 0.65f;
//        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
//        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

//        trail.emitting = false;
//        isDashing = false;
//    }

//    void Update()
//    {
//        if (Input.GetMouseButtonUp(0))
//        {
//            TryDash();
//        }
//    }

//    void FixedUpdate()
//    {
//        HandleFallDeath();
//    }

//    void LateUpdate()
//    {
//        ClampVerticalPosition();
//        CheckHorizontalBounds();
//    }

//    void TryDash()
//    {
//        if (!energy.CanDash())
//            return;

//        Vector2 dir = dashInput.direction;
//        if (dir == Vector2.zero)
//            return;

//        energy.ConsumeDashEnergy();

//        rb.linearVelocity = Vector2.zero;
//        rb.AddForce(dir * dashForce, ForceMode2D.Impulse);

//        RotateTowards(dir);

//        isDashing = true;
//        trail.emitting = true;

//        Invoke(nameof(EndDash), dashDuration);
//    }

//    void EndDash()
//    {
//        isDashing = false;
//        trail.emitting = false;
//    }

//    void RotateTowards(Vector2 dir)
//    {
//        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
//        rb.SetRotation(angle);
//    }

//    void ClampVerticalPosition()
//    {
//        Vector3 pos = transform.position;
//        pos.y = Mathf.Clamp(pos.y, Ymin, Ymax);
//        transform.position = pos;
//    }

//    void CheckHorizontalBounds()
//    {
//        float x = transform.position.x;

//        if (x < Xmin || x > Xmax)
//        {
//            Die();
//        }
//    }

//    void HandleFallDeath()
//    {
//        if (rb.linearVelocity.y >= -0.1f)
//            return;

//        if (energy.currentEnergy <= energy.maxEnergy * lowEnergyThreshold)
//        {
//            Die();
//        }
//    }

//    public void Die()
//    {
//        gameObject.SetActive(false);
//    }

////#if UNITY_EDITOR
////    void OnDrawGizmosSelected()
////    {
////        Gizmos.color = Color.green;
////        Gizmos.DrawLine(new Vector3(Xmin, Ymin), new Vector3(Xmax, Ymin));
////        Gizmos.DrawLine(new Vector3(Xmin, Ymax), new Vector3(Xmax, Ymax));

////        Gizmos.color = Color.red;
////        Gizmos.DrawLine(new Vector3(Xmin, Ymin), new Vector3(Xmin, Ymax));
////        Gizmos.DrawLine(new Vector3(Xmax, Ymin), new Vector3(Xmax, Ymax));
////    }
////#endif
//}








using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(TrailRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashForce = 15f;
    public float energyCostPerDash = 30f;

    [Header("Energy")]
    public float maxEnergy = 100f;
    public float currentEnergy = 100f;

    [Header("Physics")]
    public float gravityScale = 0.65f;
    public float airDrag = 2f;

    [Header("Score")]
    public float distanceScore;

    Rigidbody2D rb;
    TrailRenderer trail;

    Vector2 pointerStart;
    Vector2 pointerCurrent;
    bool isDragging;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();

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
        if (currentEnergy < energyCostPerDash)
            return;

        Vector2 dragDirection = pointerCurrent - pointerStart;

        // We ONLY care about Y axis
        float yDirection = Mathf.Sign(dragDirection.y);

        if (yDirection == 0)
            return;

        currentEnergy -= energyCostPerDash;

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
        // Score based on time survived (simple & effective)
        distanceScore += Time.deltaTime * 10f;
    }

    Vector2 ScreenToWorld(Vector2 screenPos)
    {
        Vector3 world = Camera.main.ScreenToWorldPoint(screenPos);
        return new Vector2(world.x, world.y);
    }
}
