using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        IdleFalling,
        Dashing,
        Depleted,
    }

    public PlayerState currentState;

    public GameObject ringPrefab;
    public CameraFollow cameraFollow;
    public GameObject outlineObject;

    private float energyCost = 10f;
    public Slider energyBar;

    Rigidbody2D rb;
    TrailRenderer trail;

    Vector2 dragStart;
    Vector2 dragEnd;

    bool isDragging;
    bool isBlockedByYellow;
    bool isOnPlatform;
    bool isSupported;

    private float minDashForce = 10f;
    private float maxDashForce = 20f;
    private float dragSensitivity = 0.8f;

    private float gravityScale = 0.65f;
    private float gravityRotateSpeed = 0.5f;
    private float airResistance = 5f;

    private float minY = -12f;
    private float maxY = 3.5f;
    private float deathY = -12f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();

        rb.gravityScale = gravityScale;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.linearDamping = 0f;

        trail.emitting = false;
        currentState = PlayerState.IdleFalling;
    }

    void Start()
    {
        GameManager.instance.currentEnergy = GameManager.instance.maxEnergy;

        if (energyBar)
        {
            energyBar.maxValue = GameManager.instance.maxEnergy;
            energyBar.value = GameManager.instance.currentEnergy;
        }

        UpdateVisualState();
    }

    void Update()
    {
        if (GameManager.instance.currentState != GameManager.GameState.Running)
            return;

        HandleInput();
        EnergyDrain();
        RefillEnergy();
        UpdateEnergyUI();
        CheckDeath();

        GameManager.instance.playerBlocked = isBlockedByYellow;
    }

    void FixedUpdate()
    {
        if (GameManager.instance.currentState != GameManager.GameState.Running)
            return;

        ApplyAirResistance();
    }

    void HandleInput()
    {
        if (currentState == PlayerState.Depleted)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            dragStart = ScreenToWorld(Input.mousePosition);
            isDragging = true;
        }

        if (Input.GetMouseButton(0) && isDragging)
            dragEnd = ScreenToWorld(Input.mousePosition);

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Dash();
            isDragging = false;
        }
    }

    void Dash()
    {
        if (GameManager.instance.currentEnergy <= 0f)
        {
            currentState = PlayerState.Depleted;
            UpdateVisualState();
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 dragDirection = dragEnd - dragStart;
        if (dragDirection.x <= 0f)
            return;

        currentState = PlayerState.Dashing;
        UpdateVisualState();
        isBlockedByYellow = false;

        float force = Mathf.Clamp(
            dragDirection.magnitude * dragSensitivity,
            minDashForce,
            maxDashForce
        );

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dragDirection.normalized * force, ForceMode2D.Impulse);

        if (ringPrefab)
            Instantiate(ringPrefab, transform.position, Quaternion.identity);

        trail.emitting = true;
        Invoke(nameof(StopTrail), 0.15f);

        if (cameraFollow)
            cameraFollow.Shake();
    }

    void EnergyDrain()
    {
        if (rb.linearVelocity.magnitude < 0.1f)
            return;

        GameManager.instance.currentEnergy -= energyCost * Time.deltaTime;
        GameManager.instance.currentEnergy = Mathf.Clamp(
            GameManager.instance.currentEnergy,
            0f,
            GameManager.instance.maxEnergy
        );

        if (GameManager.instance.currentEnergy <= 0f)
        {
            currentState = PlayerState.Depleted;
            UpdateVisualState();
        }
    }

    void ApplyAirResistance()
    {
        rb.linearVelocity = new Vector2(
            Mathf.MoveTowards(rb.linearVelocity.x, 0f, airResistance * Time.fixedDeltaTime),
            rb.linearVelocity.y
        );
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        EvaluateSupport(collision);

        if (collision.gameObject.CompareTag("Platform"))
            isOnPlatform = true;

        if (collision.gameObject.CompareTag("Yellow_wall_box") && rb.linearVelocity.x < 5f)
            isBlockedByYellow = true;

        if (currentState == PlayerState.Dashing)
        {
            currentState = PlayerState.IdleFalling;
            UpdateVisualState();
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        EvaluateSupport(collision);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isSupported = false;

        if (collision.gameObject.CompareTag("Platform"))
            isOnPlatform = false;

        if (collision.gameObject.CompareTag("Yellow_wall_box"))
            isBlockedByYellow = false;
    }

    void EvaluateSupport(Collision2D collision)
    {
        foreach (ContactPoint2D c in collision.contacts)
        {
            if (c.normal.y > 0.5f)
            {
                isSupported = true;
                return;
            }
        }

        isSupported = false;
    }

    void LateUpdate()
    {
        transform.position = new Vector3(
            transform.position.x,
            Mathf.Clamp(transform.position.y, minY, maxY),
            transform.position.z
        );

        if (currentState == PlayerState.Dashing && rb.linearVelocity.magnitude < minDashForce)
        {
            currentState = PlayerState.IdleFalling;
            UpdateVisualState();
        }

        RotateTowardsDominantForce();
    }

    void RotateTowardsDominantForce()
    {
        if (currentState == PlayerState.Dashing && rb.linearVelocity.magnitude > 0.1f)
        {
            transform.right = rb.linearVelocity.normalized;
            return;
        }

        if (isSupported)
            return;

        transform.right = Vector2.Lerp(
            transform.right,
            Vector2.down,
            gravityRotateSpeed * Time.deltaTime
        );
    }

    void RefillEnergy()
    {
        if (!isOnPlatform || GameManager.instance.currentEnergy >= GameManager.instance.maxEnergy)
            return;

        GameManager.instance.currentEnergy += 40f * Time.deltaTime;

        if (currentState == PlayerState.Depleted && GameManager.instance.currentEnergy > 0f)
        {
            currentState = PlayerState.IdleFalling;
            UpdateVisualState();
        }
    }

    void UpdateVisualState()
    {
        if (outlineObject)
            outlineObject.SetActive(currentState == PlayerState.IdleFalling);
    }

    void StopTrail()
    {
        trail.emitting = false;
    }

    void CheckDeath()
    {
        if (transform.position.y < deathY)
        {
            rb.linearVelocity = Vector2.zero;
            trail.emitting = false;
            GameManager.instance.GameOver();
            Destroy(gameObject);
        }
    }

    void UpdateEnergyUI()
    {
        if (energyBar)
            energyBar.value = GameManager.instance.currentEnergy;
    }

    Vector2 ScreenToWorld(Vector2 screenPos)
    {
        return Camera.main.ScreenToWorldPoint(screenPos);
    }

    public float GetCurrentSpeed()
    {
        return rb.linearVelocity.magnitude;
    }

    public bool IsDragging()
    {
        return isDragging;
    }
}
