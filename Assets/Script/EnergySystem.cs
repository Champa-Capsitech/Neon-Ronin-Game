using UnityEngine;

public class EnergySystem : MonoBehaviour
{
    public float maxEnergy = 100f;
    public float currentEnergy;
    public float dashCost = 30f;

    void Start()
    {
        currentEnergy = maxEnergy;
    }

    public bool CanDash()
    {
        return currentEnergy >= dashCost;
    }

    public void ConsumeDashEnergy()
    {
        currentEnergy -= dashCost;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
    }

    public void RefillFull()
    {
        currentEnergy = maxEnergy;
    }
}
