using System;
using Plugins.SableGamesAds.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace SableGamesAds.Scripts.Dummy
{
  public class InterstitialDummy : IInterstitial
  {
    public InterstitialLaunchCondition IsInterstitialCanLaunch { get; set; }
    public Action InterstitialLaunched { get; set; }

    public bool InterstitialIsLoaded() => 
      true;

    public void RequestInterstitial(bool isSplash) => 
      Debug.LogWarning("[IAds] You are using Dummy class now");

    public void ShowInterstitial(string targetScreen, UnityAction onEnd, UnityAction onFail, bool skipRestrictions = false)
    {
      InterstitialLaunched?.Invoke();
      onEnd?.Invoke();
      Debug.LogWarning("[IAds] You are using Dummy class now");
    }

    public void SetEnabled(bool value) => 
      Debug.LogWarning("[IAds] You are using Dummy class now");
  }
}