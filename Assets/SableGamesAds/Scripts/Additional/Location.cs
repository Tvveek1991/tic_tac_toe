using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SableGamesAds.Scripts.Additional
{
  public class Location
  {
    private LocationDefiner _locationDefiner;
    
    public void RequestForCode(Action<string> onComplete)
    {
      _locationDefiner = new GameObject() {name = "Location Definer Mono"}.AddComponent<LocationDefiner>();
      _locationDefiner.Launch(code =>
      {
        onComplete?.Invoke(code);
        Debug.Log($"[Location] {code}");
        if (_locationDefiner != null)
          Object.Destroy(_locationDefiner.gameObject);
      });
    }
  }
}