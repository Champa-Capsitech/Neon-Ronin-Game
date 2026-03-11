using UnityEngine;
using UnityEngine.UI;

public class EnergyBarUI : MonoBehaviour
{
    public PlayerController player;
    public Slider slider;

    private float lowEnergyThreshold = 25f;
    public Color lowEnergyColor;

    private Image fillImage;
    private Color normalColor;

    void Start()
    {
        if (player == null)
        {
            return;
        }

        if (slider == null)
        {
            return;
        }

        fillImage = slider.fillRect.GetComponent<Image>();
        normalColor = fillImage.color;

        slider.maxValue = GameManager.instance.maxEnergy;
        slider.value = GameManager.instance.currentEnergy;
    }

    void Update()
    {
        if (player == null || slider == null)
            return;

        slider.value = GameManager.instance.currentEnergy;

        // if (slider.value <= lowEnergyThreshold)
        //     fillImage.color = lowEnergyColor;
        // else
        fillImage.color = normalColor;
    }
}
