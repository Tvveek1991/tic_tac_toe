using System;
using GoogleMobileAds.Api;
using UnityEngine.Events;
#if ENABLED_ADMOB

#endif

namespace SableGamesAds.Scripts.Interfaces
{
  public interface IInterstitialBase
  {
    bool InterstitialIsLoaded();
    void RequestInterstitial(bool isSplash);
    void ShowInterstitial(string targetScreen, UnityAction onEnd, UnityAction onFail, bool isSplash = false);
    void SetEnabled(bool value);

    InterstitialLaunchCondition IsInterstitialCanLaunch { get; set; }
    Action InterstitialLaunched { get; set; }
    
#if ENABLED_ADMOB
    void AddListenerOnPaidEvent(UnityAction<AdValueEventArgs, string, string> action);
    void RemoveListenerOnPaidEvent(UnityAction<AdValueEventArgs, string, string> action);
#endif
  }
  
  public delegate bool InterstitialLaunchCondition();
}