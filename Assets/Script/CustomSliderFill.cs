using UnityEngine;

[ExecuteAlways]
public class CustomSliderFill : MonoBehaviour
{
    [Header("Fill Target")]
    public RectTransform fillTransform;

    [Header("Fill Range")]
    public float minX = -15f;
    public float maxX = -516f;

    [Range(0f, 100f)]
    public float sliderValue = 0f;

    void Update()
    {
        UpdateFill();
    }

    public void UpdateFill()
    {
        if (GameManager.instance != null)
            sliderValue =
                GameManager.instance.currentEnergy > 10 ? GameManager.instance.currentEnergy : 0;

        if (fillTransform == null)
            return;

        float xPos = Mathf.Lerp(minX, maxX, sliderValue / 100f);

        Vector3 pos = fillTransform.localPosition;
        pos.x = xPos;
        fillTransform.localPosition = pos;
    }
}
