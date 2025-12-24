using UnityEngine;
using UnityEngine.UI;

public class EnergyBarUI : MonoBehaviour
{
    public EnergySystem energy;
    public Image fillImage;

    public Color fullColor = Color.cyan;
    public Color lowColor = Color.red;

    void Update()
    {
        float fillAmount = energy.currentEnergy / energy.maxEnergy;
        fillImage.fillAmount = fillAmount;
        fillImage.color = Color.Lerp(lowColor, fullColor, fillAmount);
    }
}

