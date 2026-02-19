using UnityEngine;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }
    public static bool IsFirebaseReady { get; private set; }

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        InitializeFirebase();
#else
        Debug.Log("ℹ Firebase disabled (Editor / non-Android build)");
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.Result == DependencyStatus.Available)
                {
                    FirebaseApp app = FirebaseApp.DefaultInstance;

                    FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

                    IsFirebaseReady = true;

                    Debug.Log("🔥 Firebase initialized successfully");

                    // App-level event (safe, one-time)
                    FirebaseAnalytics.LogEvent(
                        FirebaseAnalytics.EventAppOpen
                    );
                }
                else
                {
                    Debug.LogError(
                        "❌ Firebase dependency error: " + task.Result
                    );
                }
            });
    }
#endif
}
