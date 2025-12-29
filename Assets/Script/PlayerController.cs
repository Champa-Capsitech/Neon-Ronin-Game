using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // DASH 
    // Force applied when player dashes
    public float dashForce = 15f;

    // ENERGY
    // Maximum energy player can have
    public float maxEnergy = 100f;

    // Current energy of player
    public float currentEnergy;

    // Energy cost for one dash
    public float dashCost = 25f;

    //ENERGY UI
    // Slider used to show energy
    public Slider energySlider;

    //PHYSICS
    public float gravityScale = 0.65f;
    public float airDrag = 2f;

    //SCREEN LIMITS
    [Header("Screen Limits")]
    private float xMin = -10f;
    private float xMax = 5f;
    private float yMin = -8f;
    private float yMax = 8f;

    //PLATFORM REFILL
    public float flashDuration = 2f;
    public Color flashColor;

    SpriteRenderer spriteRenderer;
    Color originalColor;

    private bool isOnPlatform = false;

    // SCORE
    public float distanceScore;

    // COMPONENTS 
    Rigidbody2D rb;
    TrailRenderer trail;

    //INPUT
    Vector2 pointerStart;
    Vector2 pointerCurrent;

    public bool isDragging;

    private float deathY = -10f;

    
    void Awake()
    {
        // Get required components
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();

        // Physics setup
        rb.gravityScale = gravityScale;
        rb.linearDamping = airDrag;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Trail off by default
        trail.emitting = false;

        // Sprite setup
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        // Initialize energy
        currentEnergy = maxEnergy;

        // Initialize energy UI
        if (energySlider != null)
        {
            energySlider.maxValue = maxEnergy;
            energySlider.value = currentEnergy;
        }
    }

    
    void Update()
    {
        // Run only when game is running
        if (GameManager.instance.currentState != GameManager.GameState.Running)
            return;

        HandlePointerInput();
        UpdateEnergyUI();

        // Death check
        if (transform.position.y < deathY)
        {
            Debug.Log("y position is less than death limit");
            Die();
        }
    }

    
    void HandlePointerInput()
    {
        // Mouse button pressed
        if (Input.GetMouseButtonDown(0))
        {
            pointerStart = ScreenToWorld(Input.mousePosition);
            isDragging = true;
        }

        // Mouse button held
        if (Input.GetMouseButton(0) && isDragging)
        {
            pointerCurrent = ScreenToWorld(Input.mousePosition);
        }

        // Mouse button released
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            TryDash();
            isDragging = false;
        }
    }

    // DASH LOGIC
    void TryDash()
    {
        // Check if enough energy
        if (!CanDash())
        {
            Debug.Log("Energy Empty");
            Die();
            return;
        }

        // Calculate drag direction
        Vector2 dragDirection = pointerCurrent - pointerStart;

        if (dragDirection == Vector2.zero)
            return;

        // Normalize direction
        Vector2 dashDirection = dragDirection.normalized;

        // Consume energy
        ConsumeDashEnergy();

        // Apply dash force
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dashDirection * dashForce, ForceMode2D.Impulse);

        // Enable trail briefly
        trail.emitting = true;
        Invoke(nameof(StopTrail), 0.15f);
    }
    private void OnDestroy()
    {
        Debug.Log("player got destroyed");
    }
    void StopTrail()
    {
        trail.emitting = false;
    }

    //ENERGY LOGIC
    bool CanDash()
    {
        return currentEnergy >= dashCost;
    }

    void ConsumeDashEnergy()
    {
        currentEnergy -= dashCost;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
    }

    void RefillFullEnergy()
    {
        currentEnergy = maxEnergy;
    }

    void UpdateEnergyUI()
    {
        if (energySlider != null)
        {
            energySlider.value = currentEnergy;
        }
    }

    //SCREEN LIMIT
    void LateUpdate()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, xMin, xMax);
        pos.y = Mathf.Clamp(pos.y, yMin, yMax);

        transform.position = pos;
    }

    //COLLISION
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Platform"))
            return;

        Debug.Log("Player collided with platform");

        isOnPlatform = true;
        RefillFullEnergy();
        //StartCoroutine(FlashPlayer());
    }

    //DEATH
    public void Die()
    {
        rb.linearVelocity = Vector2.zero;
        GameManager.instance.GameOver();
    }

    //UTILITY
    Vector2 ScreenToWorld(Vector2 screenPos)
    {
        Vector3 world = Camera.main.ScreenToWorldPoint(screenPos);
        return new Vector2(world.x, world.y);
    }

    // ================= OPTIONAL FLASH =================
    //IEnumerator FlashPlayer()
    //{
    //    spriteRenderer.color = flashColor;
    //    yield return new WaitForSeconds(flashDuration);
    //    spriteRenderer.color = originalColor;
    //}
}





