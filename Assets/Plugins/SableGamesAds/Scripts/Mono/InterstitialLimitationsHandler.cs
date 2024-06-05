using System;

namespace SableGamesAds.Scripts.Mono
{
  public class InterstitialLimitationsHandler : Singleton<InterstitialLimitationsHandler>
  {
    public Action OnLateUpdate;
    
    private void LateUpdate() => 
      OnLateUpdate?.Invoke();
  }
}