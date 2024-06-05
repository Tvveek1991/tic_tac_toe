using System.Collections;
using UnityEngine;

namespace SableGamesRemote.Scripts.Tools
{
  public class RemoteSwitcher : MonoBehaviour, IRemoteSwitcher
  {
    private const string TestCounterKey = "RemoteSwitcher.TestModeCounter";

    private bool _isBlocked;
    
    private void Awake() => 
      StartCoroutine(Block());

    private IEnumerator Block()
    {
      yield return new WaitForSecondsRealtime(7);
      _isBlocked = true;
    }

    public int TestModeCounter
    {
      get => PlayerPrefs.GetInt(TestCounterKey, 5);
      private set => PlayerPrefs.SetInt(TestCounterKey, value);
    }
    
    public void OnGUI()
    {
      if (TestModeCounter == 0 || _isBlocked) 
        return;
      
      if (GUI.Button(new Rect(0, 0, 200, 200), "", GUIStyle.none))
      {
        TestModeCounter--;

        if(TestModeCounter == 0)
          ShowAndroidToastMessage("Следующий запуск произойдет с тестовой версией!");
      }
    }
    
    /// <param name="message">Message string to show in the toast.</param>
    private void ShowAndroidToastMessage(string message)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
      AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
      AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

      if (unityActivity != null)
      {
        AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
        unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
          AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
          toastObject.Call("show");
        }));
      }
#else
      Debug.Log(message);
#endif
    }
  }
}
