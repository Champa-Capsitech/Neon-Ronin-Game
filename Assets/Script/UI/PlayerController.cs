using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public GameObject ringPrefab;

    public CameraFollow cameraFollow;

    private float minDashForce = 6f;
    private float maxDashForce = 15f;
    private float dragSensitivity = 0.8f;

    //Energy 
    public float maxEnergy = 100f;

    public float currentEnergy;
    public float energyCost = 10f;
    public Slider energyBar;

    //Physics 
    private float gravityScale = 0.65f;
    private float airResistance = 5f;

    //Limits 
    private float minY = -12f;
    private float maxY = 3.5f;
    private float deathY = -12f;

    Rigidbody2D rb;
    TrailRenderer trail;

    //Input
    Vector2 dragStart;
    Vector2 dragEnd;
    bool isDragging;

    //State
    bool isBlockedByYellow;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();

        rb.gravityScale = gravityScale;
        rb.linearDamping = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        trail.emitting = false;

        currentEnergy = maxEnergy;
        if (energyBar != null)
        {
            energyBar.maxValue = maxEnergy;
            energyBar.value = currentEnergy;
        }
        }
    

    void Update()
    {
        if (GameManager.instance.currentState != GameManager.GameState.Running)
            return;

        HandleInput();
        EnergyDrain();
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

        if (Input.GetMouseButtonDown(0))
        {
            dragStart = ScreenToWorld(Input.mousePosition);
            isDragging = true;
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            dragEnd = ScreenToWorld(Input.mousePosition);
            Vector2 dragDir = dragEnd - dragStart;

            if (dragDir != Vector2.zero)
                RotateTowardsDrag(dragDir);
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Dash();
            isDragging = false;
        }
    }

    void Dash()
    {
        if (currentEnergy <= 0f)
        {
            Die2();
            return;
        }

        Vector2 dragDirection = dragEnd - dragStart;

        if (dragDirection.x <= 0f)
            return;

        isBlockedByYellow = false;

        float dragLength = dragDirection.magnitude;
        float dynamicForce = dragLength * dragSensitivity;
        dynamicForce = Mathf.Clamp(dynamicForce, minDashForce, maxDashForce);

        Vector2 direction = dragDirection.normalized;

        Vector3 dashStartPosition = transform.position;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * dynamicForce, ForceMode2D.Impulse);

        if (ringPrefab != null)
        {
            Instantiate(ringPrefab, dashStartPosition, Quaternion.identity);
        }

        trail.emitting = true;
        Invoke(nameof(StopTrail), 0.15f);
        if (cameraFollow != null)
        {
            cameraFollow.Shake();
        }
    }


    void EnergyDrain()
    {
        if (rb.linearVelocity.magnitude < 0.1f)
            return;
        if (currentEnergy <= 0f)
        {
            currentEnergy = 0f;
            Die();
            return;
        }
        currentEnergy -= energyCost * Time.deltaTime;
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
    }

    void ApplyAirResistance()
    {
        Vector2 velocity = rb.linearVelocity;

        velocity.x = Mathf.MoveTowards(
            velocity.x,
            0f,
            airResistance * Time.fixedDeltaTime
        );

        rb.linearVelocity = velocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            RefillEnergy();
        }

        if (collision.gameObject.CompareTag("Yellow_wall_box"))
        {
            if (rb.linearVelocity.x <= 0.1f)
                isBlockedByYellow = true;

            RefillEnergy();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Yellow_wall_box"))
            isBlockedByYellow = false;
    }

    void RotateTowardsDrag(Vector2 direction)
    {
        transform.up = direction;
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }

    void CheckDeath()
    {
        if (transform.position.y < deathY)
            Die();
    }

    void StopTrail()
    {
        trail.emitting = false;
    }

    void UpdateEnergyUI()
    {
        if (energyBar != null)
            energyBar.value = currentEnergy;
    }

    void RefillEnergy()
    {
        currentEnergy = maxEnergy;
        if (energyBar != null)
            energyBar.value = currentEnergy;
    }

    public void Die()
    {
        rb.linearVelocity = Vector2.zero;
        GameManager.instance.GameOver();
    }

    public void Die2()
    {
        rb.linearVelocity = Vector2.zero;
    }

    Vector2 ScreenToWorld(Vector2 screenPos)
    {
        Vector3 world = Camera.main.ScreenToWorldPoint(screenPos);
        return new Vector2(world.x, world.y);
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