using UnityEngine;

public class VibrationHelper
{
    public static void Vibrate(long duration)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass pluginClass = new AndroidJavaClass("com.radius.system.vibrationplugin.VibrationHelper"))
        {
            using (AndroidJavaObject activity = GetUnityActivity())
            {
                pluginClass.CallStatic("vibrate", activity, duration);
            }
        }
#endif
    }

    private static AndroidJavaObject GetUnityActivity()
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    }
}