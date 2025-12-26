using UnityEngine;
using UnityEngine.UI;

public class EnergyBarUI : MonoBehaviour
{
    public EnergySystem energy;
    public Slider slider;

    void Start()
    {
        slider.maxValue = energy.maxEnergy;
        slider.value = energy.currentEnergy;
    }

    void Update()
    {
        slider.value = energy.currentEnergy; // 🔥 SAME AS: slider.value = health
    }
}
