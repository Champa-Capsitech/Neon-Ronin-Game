//using UnityEngine;

//public class EnergySystem : MonoBehaviour
//{
//    public float maxEnergy = 100f;
//    public float currentEnergy;

//    public float dashCost = 25f;

//    void Awake()
//    {
//        currentEnergy = maxEnergy;
//    }

//    public bool CanDash()
//    {
//        return currentEnergy >= dashCost;
//    }

//    public void ConsumeDashEnergy()
//    {
//        currentEnergy -= dashCost;
//        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
//    }

//    public void RefillFull()
//    {
//        currentEnergy = maxEnergy;
//    }
//}










//using UnityEngine;

//public class EnergySystem : MonoBehaviour
//{
//    public float maxEnergy = 100f;
//    public float currentEnergy;
//    public float dashCost = 25f;

//    void Awake()
//    {
//        currentEnergy = maxEnergy;
//    }

//    public bool CanDash()
//    {
//        return currentEnergy >= dashCost;
//    }

//    public void ConsumeDashEnergy()
//    {
//        currentEnergy -= dashCost;
//        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
//    }

//    public void RefillFull()
//    {
//        currentEnergy = maxEnergy;
//    }

//    // 👇 UI will use this
//    public float GetEnergyPercent()
//    {
//        return currentEnergy / maxEnergy;
//    }
//}





using UnityEngine;

public class EnergySystem : MonoBehaviour
{
    [Header("Energy Settings")]
    public float maxEnergy = 100f;
    public float currentEnergy;

    public float dashCost = 30f;

    void Awake()
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

