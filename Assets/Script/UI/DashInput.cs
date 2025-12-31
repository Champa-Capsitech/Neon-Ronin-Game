using UnityEngine;

public class DashInput : MonoBehaviour
{
    public Vector2 startPoint;
    public Vector2 currentPoint;
    public Vector2 direction;

    public bool isHolding;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isHolding = true;
        }

        if (Input.GetMouseButton(0) && isHolding)
        {
            currentPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction = (currentPoint - startPoint).normalized;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isHolding = false;
        }
    }
}
