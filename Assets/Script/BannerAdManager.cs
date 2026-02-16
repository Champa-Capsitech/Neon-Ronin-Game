using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BannerAdManager : MonoBehaviour
{
    public static BannerAdManager Instance;

    private BannerView bannerView;

#if UNITY_ANDROID
    public string bannerAdUnitId = "ca-app-pub-8530302013109448/3496815230";
#elif UNITY_IOS || UNITY_IPHONE
    public string bannerAdUnitId = "ca-app-pub-8530302013109448/8916199853";
#else
    private string bannerAdUnitId = "unused";
#endif

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("bannerAdUnitId" + bannerAdUnitId);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        MobileAds.Initialize(initStatus =>
        {
            LoadBanner();
        });

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            LoadBanner();
            ShowBanner();
        }
        else
        {
            HideBanner();
        }
    }

    public void LoadBanner()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        AdSize customSize = new AdSize(468, 60); //(728, 90)
        bannerView = new BannerView(bannerAdUnitId, customSize, AdPosition.Bottom);

        AdRequest request = new AdRequest();
        bannerView.LoadAd(request);
    }

    public void ShowBanner()
    {
        if (bannerView != null)
            bannerView.Show();
    }

    public void HideBanner()
    {
        if (bannerView != null)
            bannerView.Hide();
    }

    void OnDestroy()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
