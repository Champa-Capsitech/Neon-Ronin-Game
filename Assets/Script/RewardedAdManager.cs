using System;
using GoogleMobileAds.Api;
using UnityEngine;

public class RewardedAdManager : MonoBehaviour
{
    public static RewardedAdManager Instance;

    private RewardedAd rewardedAd;
    private Action rewardCallback;
    private bool rewardEarned;

#if UNITY_ANDROID
    public string rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IOS || UNITY_IPHONE
    public string rewardedAdUnitId = "ca-app-pub-8530302013109448/8140713739";
#else
    private string rewardedAdUnitId = "unexpected_platform";
#endif

    // public string rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917"; //testing
    // public string rewardedAdUnitId = "ca-app-pub-8530302013109448/4518432412"; //real

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadRewarded();
    }

    public void LoadRewarded()
    {
        rewardEarned = false;

        AdRequest request = new AdRequest();

        RewardedAd.Load(
            rewardedAdUnitId,
            request,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.Log("Rewarded load failed");
                    return;
                }

                rewardedAd = ad;
                Debug.Log("Rewarded loaded");

                rewardedAd.OnAdFullScreenContentClosed += () =>
                {
                    if (rewardEarned)
                    {
                        rewardCallback?.Invoke();
                    }

                    rewardCallback = null;
                    LoadRewarded();
                };
            }
        );
    }

    public void ShowRewarded(Action onReward, Action NoAd)
    {
        if (rewardedAd == null || !rewardedAd.CanShowAd())
        {
            Debug.Log("Rewarded not ready");
            NoAd?.Invoke();
            return;
        }

        rewardCallback = onReward;

        rewardedAd.Show(reward =>
        {
            rewardEarned = true;
            Debug.Log("Reward earned");
        });
    }

    // internal void ShowRewarded(Action value)
    // {
    //     throw new NotImplementedException();
    // }
}
