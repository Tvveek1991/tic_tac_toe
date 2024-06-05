using System;
using UnityEngine.Events;

namespace Plugins.SableGamesAds.Scripts.Interfaces
{
  public interface IInterstitialBase
  {
    bool InterstitialIsLoaded();
    void RequestInterstitial(bool isSplash);
    void ShowInterstitial(string targetScreen, UnityAction onEnd, UnityAction onFail, bool isSplash = false);
    void SetEnabled(bool value);

    InterstitialLaunchCondition IsInterstitialCanLaunch { get; set; }
    Action InterstitialLaunched { get; set; }
  }
  
  public delegate bool InterstitialLaunchCondition();
}