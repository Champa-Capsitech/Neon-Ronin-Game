using System;
using GoogleMobileAds.Api;
using UnityEngine;

public class InterstitialAdManager : MonoBehaviour
{
    public static InterstitialAdManager Instance;

    private InterstitialAd interstitialAd;
    private bool isShowingAd = false;
    private bool adLoaded = false;
    // public string interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
    // public string interstitialAdUnitId = "ca-app-pub-8530302013109448/7789359949";

#if UNITY_ANDROID
    public string interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IOS || UNITY_IPHONE
    public string interstitialAdUnitId = "ca-app-pub-8530302013109448/8024339254";
#else
    private string interstitialAdUnitId = "unexpected_platform";
#endif

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        MobileAds.Initialize(initStatus =>
        {
            LoadInterstitial();
        });
    }

    public void LoadInterstitial()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        adLoaded = false;

        InterstitialAd.Load(
            interstitialAdUnitId,
            new AdRequest(),
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Interstitial failed to load: " + error);
                    return;
                }

                interstitialAd = ad;
                adLoaded = true;
                RegisterCallbacks();

                Debug.Log("Interstitial loaded");
            }
        );
    }

    private void RegisterCallbacks()
    {
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            isShowingAd = true;
            Debug.Log("Interstitial opened");
        };

        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            isShowingAd = false;
            Debug.Log("Interstitial closed");
            LoadInterstitial();
        };

        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            isShowingAd = false;
            Debug.LogError("Interstitial failed to show: " + error);
            LoadInterstitial();
        };
    }

    public void ShowInterstitialIfReady()
    {
        if (interstitialAd == null || !interstitialAd.CanShowAd())
        {
            Debug.Log("Interstitial not ready");
            LoadInterstitial();
            return;
        }

        interstitialAd.Show();
    }

    void OnDestroy()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }
    }
}
