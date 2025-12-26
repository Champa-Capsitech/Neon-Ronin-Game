using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(TrailRenderer))]
public class PlayerController : MonoBehaviour
{
    
    public float dashForce = 15f;

    //  ENERGY
    
    public float maxEnergy = 100f;
    public float currentEnergy;
    public float dashCost = 25f;

    //  ENERGY UI
    
    public Slider energySlider;

    
    public float gravityScale = 0.65f;
    public float airDrag = 2f;

    
    public float flashDuration = 2f;
    public Color flashColor;
    SpriteRenderer spriteRenderer;
    Color originalColor;
    private bool isOnPlatform = false;


    public float distanceScore;

    Rigidbody2D rb;
    TrailRenderer trail;

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

        //  INIT ENERGY
        currentEnergy = maxEnergy;

        //  INIT UI
        if (energySlider != null)
        {
            energySlider.maxValue = maxEnergy;
            energySlider.value = currentEnergy;
        }
    }

    void Update()
    {
        //Debug.Log("currentEnergy" + currentEnergy);
        //Debug.Log("isOnPlatform" + isOnPlatform);
        if (GameManager.instance.currentState != GameManager.GameState.Running)
            return;

        HandlePointerInput();
        UpdateEnergyUI();

        if (transform.position.y < deathY)
        {
            Debug.Log("y position is less then -10");
            Die();
        }
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
        if (!CanDash())
        {
            Die();
            Debug.Log("Energy : 0");
            return;
        }

        Vector2 dragDirection = pointerCurrent - pointerStart;

        // ONLY Y AXIS MOVEMENT
        float yDirection = Mathf.Sign(dragDirection.y);
        if (yDirection == 0)
            return;

        ConsumeDashEnergy(); //  ENERGY DECREASES HERE

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(Vector2.up * yDirection * dashForce, ForceMode2D.Impulse);

        trail.emitting = true;
        Invoke(nameof(StopTrail), 0.15f);
    }

    void StopTrail()
    {
        trail.emitting = false;
    }

    public void Die()
    {
        rb.linearVelocity = Vector2.zero;
        GameManager.instance.GameOver();
        //gameObject.SetActive(false);
    }

    Vector2 ScreenToWorld(Vector2 screenPos)
    {
        Vector3 world = Camera.main.ScreenToWorldPoint(screenPos);
        return new Vector2(world.x, world.y);
    }

    //  ENERGY LOGIC (MERGED FROM EnergySystem)

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Plateform"))
            return;

        Debug.Log("Player collide with platform");

        isOnPlatform = true;
        RefillFullEnergy();
        //StartCoroutine(FlashPlayer());
    }

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Plateform"))
    //    {
    //        isOnPlatform = false;
    //    }
    //}

    //IEnumerator FlashPlayer()
    //{
    //    spriteRenderer.color = flashColor;
    //    yield return new WaitForSeconds(flashDuration);
    //    spriteRenderer.color = originalColor;
    //}
}
