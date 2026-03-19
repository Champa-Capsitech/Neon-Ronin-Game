using System;
using System.Collections;
using System.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VersionChecker : MonoBehaviour
{
    FirebaseFirestore db;

    public string currentVersion;

    [SerializeField]
    private GameObject UpdatePanel;

    [SerializeField]
    private Button updateBtn;

    [SerializeField]
    private Button SkipBtn;

    [SerializeField]
    private TMP_Text LoadingTxt;

    private void Awake()
    {
        FirebaseApp
            .CheckAndFixDependenciesAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.Result == DependencyStatus.Available)
                {
                    db = FirebaseFirestore.DefaultInstance;
                    Debug.Log("Firestore Ready");
                    CheckVersion();
                }
                else
                {
                    Debug.LogError("Firebase not ready");
                }
            });
    }

    public void CheckVersion()
    {
        LoadingTxt.gameObject.SetActive(true);

        if (UpdatePanel.activeSelf)
        {
            StartCoroutine(
                SafeFirestoreCall<DocumentSnapshot>(
                    () => db.Collection("AppConfig").Document("version").GetSnapshotAsync(),
                    snapshot =>
                    {
                        LoadingTxt.gameObject.SetActive(false);

                        if (snapshot.Exists)
                        {
                            string latestVersion = snapshot.GetValue<string>("latestVersion");
                            bool forceUpdate = snapshot.GetValue<bool>("forceUpdate");

                            CompareVersions(currentVersion, latestVersion, forceUpdate);
                        }
                        else
                        {
                            AndroidToast.ShowToast("Can`t fetch the latest version");
                            GoToHome();
                        }
                    },
                    () =>
                    {
                        LoadingTxt.gameObject.SetActive(false);
                        AndroidToast.ShowToast("Can`t fetch the latest version");
                        GoToHome();
                    }
                )
            );
        }
    }

    void CompareVersions(string current, string latest, bool force)
    {
        if (current != latest)
        {
            if (force)
            {
                LoadingTxt.gameObject.SetActive(false);
                ShowForceUpdatePopup();
            }
            else
            {
                LoadingTxt.gameObject.SetActive(false);
                ShowOptionalUpdatePopup();
            }
        }
        else
        {
            LoadingTxt.gameObject.SetActive(false);
        }
    }

    public void ShowForceUpdatePopup()
    {
        if (UpdatePanel != null)
        {
            UpdatePanel.SetActive(true);
            SkipBtn.gameObject.SetActive(false);
        }
    }

    public void ShowOptionalUpdatePopup()
    {
        if (UpdatePanel != null)
        {
            UpdatePanel.SetActive(true);
        }
    }

    public void GoToHome()
    {
        Debug.Log("Go Home Called");
        GameManager.instance.SetState(GameManager.GameState.Start);
    }

    IEnumerator SafeFirestoreCall<T>(
        Func<Task<T>> firestoreTask,
        Action<T> onSuccess,
        Action onFail,
        float timeout = 10f
    )
    {
        bool isCompleted = false;
        bool isSuccess = false;
        T result = default;

        var task = firestoreTask();

        task.ContinueWithOnMainThread(t =>
        {
            isCompleted = true;

            if (t.IsCompleted && !t.IsFaulted && !t.IsCanceled)
            {
                result = t.Result;
                isSuccess = true;
            }
        });

        float timer = 0f;

        while (!isCompleted && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (!isCompleted || !isSuccess)
        {
            onFail?.Invoke();
        }
        else
        {
            onSuccess?.Invoke(result);
        }
    }
}
