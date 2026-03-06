using GoogleMobileAds.Api;
using UnityEngine;

public class AdInitializer : MonoBehaviour
{
    void Awake()
    {
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("AdMob Initialized Successfully");
        });
    }
}
