using UnityEngine;
using UnityEngine.UI;

public class EnergyBarUI : MonoBehaviour
{
    public PlayerController player; 
    public Slider slider; 

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("EnergyBarUI: PlayerController NOT assigned");
            return;
        }

        if (slider == null)
        {
            Debug.LogError("EnergyBarUI: Slider NOT assigned");
            return;
        }

        slider.maxValue = GameManager.instance.maxEnergy;
        slider.value = GameManager.instance.currentEnergy;
    }

    void Update()
    {
        if (player == null || slider == null)
            return;

        slider.value = GameManager.instance.currentEnergy;
    }
}

