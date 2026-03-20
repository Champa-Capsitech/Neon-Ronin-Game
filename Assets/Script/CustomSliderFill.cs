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
    private bool energyEmptyLogged = false;

    void Update()
    {
        UpdateFill();
    }

    public void UpdateFill()
    {
        if (GameManager.instance != null)
        {
            float energy = GameManager.instance.currentEnergy;

            sliderValue = energy > 10 ? energy : 0;

            if (energy <= 10 && !energyEmptyLogged)
            {
                AnalyticsLogger.LogEnergyEmpty();
                energyEmptyLogged = true;
            }

            if (energy > 10)
            {
                energyEmptyLogged = false;
            }
        }
        if (fillTransform == null)
            return;

        float xPos = Mathf.Lerp(minX, maxX, sliderValue / 100f);

        Vector3 pos = fillTransform.localPosition;
        pos.x = xPos;
        fillTransform.localPosition = pos;
    }
}
