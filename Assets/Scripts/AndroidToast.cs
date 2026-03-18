using UnityEngine;

public class AndroidToast
{
    public static void ShowToast(string message)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        activity.Call(
            "runOnUiThread",
            new AndroidJavaRunnable(() =>
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                AndroidJavaObject context = activity.Call<AndroidJavaObject>(
                    "getApplicationContext"
                );

                AndroidJavaObject toast = toastClass.CallStatic<AndroidJavaObject>(
                    "makeText",
                    context,
                    message,
                    toastClass.GetStatic<int>("LENGTH_SHORT")
                );

                toast.Call("show");
            })
        );
#endif
    }
}
