using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float dashForce = 15f;
    public float maxEnergy = 100f;
    public float currentEnergy;
    private float dashCost = 5f;

    public Slider energySlider;

    public float gravityScale = 0.65f;
    public float airDrag = 2f;

    private float xMin = -10f;
    private float xMax = 5f;
    private float yMin = -8f;
    private float yMax = 8f;

    public float flashDuration = 2f;
    public Color flashColor;

    SpriteRenderer spriteRenderer;
    Color originalColor;

    private bool isOnPlatform = false;

    public float distanceScore;

    Rigidbody2D rb;
    TrailRenderer trail;

    //INPUT
    Vector2 pointerStart;
    Vector2 pointerCurrent;

    public bool isDragging;

    private float deathY = -10f;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();

        rb.gravityScale = gravityScale;
        rb.linearDamping = airDrag;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        trail.emitting = false;

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        currentEnergy = maxEnergy;

        if (energySlider != null)
        {
            energySlider.maxValue = maxEnergy;
            energySlider.value = currentEnergy;
        }
    }


    void Update()
    {
        if (GameManager.instance.currentState != GameManager.GameState.Running)
            return;

        HandlePointerInput();
        UpdateEnergyUI();

        if (transform.position.y < deathY)
        {
            Debug.Log("y position is less than death limit");
            Die();
        }
    }


    void HandlePointerInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            pointerStart = ScreenToWorld(Input.mousePosition);
            isDragging = true;
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            pointerCurrent = ScreenToWorld(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            TryDash();
            isDragging = false;
        }
    }

    void TryDash()
    {
        if (!CanDash())
        {
            Debug.Log("Energy Empty");
            Die();
            return;
        }

        Vector2 dragDirection = pointerCurrent - pointerStart;

        if (dragDirection == Vector2.zero)
            return;

        Vector2 dashDirection = dragDirection.normalized;

        ConsumeDashEnergy();

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dashDirection * dashForce, ForceMode2D.Impulse);

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

    void LateUpdate()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, xMin, xMax);
        pos.y = Mathf.Clamp(pos.y, yMin, yMax);

        transform.position = pos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Platform"))
            return;

        Debug.Log("Player collided with platform");

        isOnPlatform = true;
        RefillFullEnergy();
        //StartCoroutine(FlashPlayer());
    }

    public void Die()
    {
        rb.linearVelocity = Vector2.zero;
        GameManager.instance.GameOver();
    }

    Vector2 ScreenToWorld(Vector2 screenPos)
    {
        Vector3 world = Camera.main.ScreenToWorldPoint(screenPos);
        return new Vector2(world.x, world.y);
    }

    //IEnumerator FlashPlayer()
    //{
    //    spriteRenderer.color = flashColor;
    //    yield return new WaitForSeconds(flashDuration);
    //    spriteRenderer.color = originalColor;
    //}
}
