using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace _Features.Disable_Ads_Window.Timer.Mono
{
  public class UrlRequest : MonoBehaviour
  {
    private const string Url = "http://worldtimeapi.org/api/ip";

    public void FetchTime(Action<string> onFetch) => 
      StartCoroutine(GetRequest(Url, onFetch));

    IEnumerator GetRequest(string uri, Action<string> onComplete)
    {
      using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
      {
        yield return webRequest.SendWebRequest();
        
        switch (webRequest.result)
        {
          case UnityWebRequest.Result.ConnectionError:
          case UnityWebRequest.Result.DataProcessingError:
            break;
          case UnityWebRequest.Result.ProtocolError:
            break;
          case UnityWebRequest.Result.Success:
            Debug.Log(webRequest.downloadHandler.text);
            break;
        }

        onComplete?.Invoke(JsonUtility.FromJson<Time>(webRequest.downloadHandler.text).utc_datetime);
      }
    }
    
    [Serializable]
    public struct Time
    {
      public string utc_datetime;
    }
  }
}