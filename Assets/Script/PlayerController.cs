using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //MOVEMENT
    public float dashForce = 15f;

    //ENERGY
    public float maxEnergy = 100f;
    public float currentEnergy;
    public float energyPerDash = 5f;
    public Slider energyBar;

    //PHYSICS
    public float gravityScale = 0.65f;
    public float airDrag = 2f;

    //LIMITS
    private float minY = -20f;
    private float maxY = 8f;

    //DEATH
    private float deathY = -20f;

    //DISTANCE SCORE
    public float distanceScore;
    private float startX;

    Rigidbody2D rb;
    TrailRenderer trail;

    //INPUT
    Vector2 dragStart;
    Vector2 dragEnd;
    bool isDragging;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();

        rb.gravityScale = gravityScale;
        rb.linearDamping = airDrag;          // keep (not obsolete)
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        trail.emitting = false;

        currentEnergy = maxEnergy;

        if (energyBar != null)
        {
            energyBar.maxValue = maxEnergy;
            energyBar.value = currentEnergy;
        }
    }

    void Start()
    {
        startX = transform.position.x; // score starts from 0
    }

    void Update()
    {
        if (GameManager.instance.currentState != GameManager.GameState.Running)
            return;

        HandleInput();
        UpdateEnergyUI();
        CheckDeath();
        UpdateDistanceScore();
    }

    //INPUT
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

            Vector2 dragDirection = dragEnd - dragStart;
            if (dragDirection != Vector2.zero)
            {
                RotateTowardsDrag(dragDirection);
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Dash();
            isDragging = false;
        }
    }

    //DASH
    void Dash()
    {
        if (currentEnergy < energyPerDash)
        {
            Die();
            return;
        }

        Vector2 dragDirection = dragEnd - dragStart;

        // Block backward movement
        if (dragDirection.x <= 0f)
            return;

        Vector2 direction = dragDirection.normalized;

        currentEnergy -= energyPerDash;                             // energyCostPerDash
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * dashForce, ForceMode2D.Impulse);

        trail.emitting = true;
        Invoke(nameof(StopTrail), 0.15f);
    }



    private void OnDestroy()
    {
        Debug.Log("player got destroyed");
    }
    
    private void OnDisable()
    {
        Debug.Log("player got disabled");
    }
    void StopTrail()
    {
        trail.emitting = false;
    }

    //ENERGY
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

    //COLLISION
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            RefillEnergy();
        }
    }

    //ROTATION
    void RotateTowardsDrag(Vector2 direction)
    {
        transform.up = direction; // triangle head faces drag
    }

    //LIMITS
    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }

    //DEATH
    void CheckDeath()
    {
        if (transform.position.y < deathY)
        {
            Die();
        }
    }

    public void Die()
    {
        rb.linearVelocity = Vector2.zero;
        GameManager.instance.GameOver();
    }

    //SCORE
    void UpdateDistanceScore()
    {
        distanceScore = transform.position.x - startX;
        distanceScore = Mathf.Max(distanceScore, 0f);
    }

    //UTILITY
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