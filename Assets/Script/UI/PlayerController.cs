using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public GameObject ringPrefab;
    public CameraFollow cameraFollow;
    public GameObject outlineObject;

    public Slider energyBar;
    private float energyCost = 0.25f;

    Rigidbody2D rb;
    TrailRenderer trail;

    Vector2 dragStart;
    Vector2 dragEnd;

    bool isDragging;
    bool isBlockedByYellow;
    bool isOnPlatform;
    bool isSupported;
    private bool inputLocked = false;

    private float minDashForce = 10f;
    private float maxDashForce = 20f;
    private float dragSensitivity = 0.8f;

    [SerializeField]
    private float minDragDistance = 0.6f;

    private float gravityScale = 0.65f;
    private float gravityRotateSpeed = 0.5f;

    private float minY = -12f;
    private float maxY = 300f;
    private float deathY = -12f;

    private float dashDuration = 0.15f;
    private float dashTimer;
    private float originalGravity;

    private bool collidedThisFrame;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();

        rb.gravityScale = gravityScale;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        //rb.linearDamping = 0f;

        originalGravity = gravityScale;

        trail.emitting = false;
    }

    void Start()
    {
        GameManager.instance.currentEnergy = GameManager.instance.maxEnergy;

        if (energyBar)
        {
            energyBar.maxValue = GameManager.instance.maxEnergy;
            energyBar.value = GameManager.instance.currentEnergy;
        }

        UpdateOutline();
    }

    void Update()
    {
        if (GameManager.instance.currentState != GameManager.GameState.Running)
            return;

        HandleInput();
        RefillEnergy();
        UpdateEnergyUI();
        CheckDeath();

        GameManager.instance.playerBlocked = isBlockedByYellow;

        UpdateOutline();

        collidedThisFrame = false;

        if (inputLocked && rb.linearVelocity.magnitude < minDashForce)
        {
            inputLocked = false;
        }
    }

    void FixedUpdate()
    {
        if (GameManager.instance.currentState != GameManager.GameState.Running)
            return;

        if (dashTimer > 0f)
        {
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
                rb.gravityScale = originalGravity;
        }
    }

    void HandleInput()
    {
        if (inputLocked)
            return;

        if (GameManager.instance.currentEnergy <= 0f)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            dragStart = ScreenToWorld(Input.mousePosition);
            dragEnd = dragStart;
            isDragging = true;
        }

        if (Input.GetMouseButton(0) && isDragging)
            dragEnd = ScreenToWorld(Input.mousePosition);

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            float dragDistance = Vector2.Distance(dragStart, dragEnd);

            if (dragDistance >= minDragDistance)
            {
                Dash();
            }

            isDragging = false;
        }
    }

    void Dash()
    {
        if (GameManager.instance.currentEnergy <= 10f)
            return;

        inputLocked = true;

        Vector2 dragDirection = dragEnd - dragStart;
        if (dragDirection.x <= 0f)
        {
            inputLocked = false;
            return;
        }

        float dashSpeed = Mathf.Clamp(
            dragDirection.magnitude * dragSensitivity,
            minDashForce,
            maxDashForce
        );

        rb.linearVelocity = dragDirection.normalized * dashSpeed;
        rb.gravityScale = 0f;
        dashTimer = dashDuration;

        GameManager.instance.currentEnergy *= (1f - energyCost);
        GameManager.instance.currentEnergy = Mathf.Clamp(
            GameManager.instance.currentEnergy,
            0f,
            GameManager.instance.maxEnergy
        );

        if (ringPrefab)
            Instantiate(ringPrefab, transform.position, Quaternion.identity);

        trail.emitting = true;
        Invoke(nameof(StopTrail), 0.15f);

        if (cameraFollow)
            cameraFollow.Shake();

        GameManager.instance.PlayDashSound();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        EvaluateSupport(collision);

        if (collision.gameObject.CompareTag("Platform"))
            isOnPlatform = true;

        if (collision.gameObject.CompareTag("Yellow_wall_box") && rb.linearVelocity.x < 5f)
            isBlockedByYellow = true;

        collidedThisFrame = true;

        rb.gravityScale = originalGravity;

        inputLocked = false;
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

        RotateTowardsDominantForce();
    }

    void RotateTowardsDominantForce()
    {
        if (rb.linearVelocity.magnitude > 0.1f)
            transform.right = rb.linearVelocity.normalized;
        else if (!isSupported)
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
    }

    void UpdateOutline()
    {
        if (!outlineObject)
            return;

        bool canDash =
            !inputLocked
            && GameManager.instance.currentState == GameManager.GameState.Running
            && GameManager.instance.currentEnergy > 10f;

        if (outlineObject.activeSelf != canDash)
            outlineObject.SetActive(canDash);
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
}
