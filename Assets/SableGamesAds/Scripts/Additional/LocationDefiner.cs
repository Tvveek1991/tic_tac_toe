using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace SableGamesAds.Scripts.Additional
{
  public class LocationDefiner : MonoBehaviour
  {
    private const int WebRequestTimeout = 5;
    
    private Action<string> _onComplete;

    public void Launch(Action<string> onComplete)
    {
      _onComplete = onComplete;
      StartCoroutine(Request());
    }

    IEnumerator Request()
    {
      UnityWebRequest webRequest = UnityWebRequest.Get("https://www.google.com/");    
      webRequest.timeout = WebRequestTimeout;
      yield return webRequest.SendWebRequest();

      if (webRequest.error != null)
      {
        _onComplete?.Invoke("default");
        yield break;
      }

      Match match = Regex.Match(webRequest.downloadHandler.text, "google.kHL='[a-zA-Z]+-?[a-zA-Z]*'");
      var code = match.Value.Replace("google.kHL='", "").Replace("'", "");
      _onComplete?.Invoke(code);
    }
  }
}