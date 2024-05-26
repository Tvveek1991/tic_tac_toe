using System;
using GoogleMobileAds.Api;
using SableGamesAds.Scripts.Data;
using SableGamesAds.Scripts.Dummy;
using SableGamesAds.Scripts.Interfaces;
using SableGamesRemote.Scripts.Data;
using UnityEngine;
using UnityEngine.Events;
#if ENABLED_ADMOB

#endif

namespace SableGamesAds
{
  public class AdsDummy : IAdsService
  {
#if !WITHOUT_BANNER
    public IBanner Banner { get; set; }
#endif
    public IRewarded Rewarded { get; set; }
    public IInterstitialBase CurrentInterstitial { get; set; }

    public bool AdsEnabled { get; set; } = false;

    public void Initialize(PlacementType placementType, Action onInitialize, bool isSplash = false)
    {
      CurrentInterstitial = new InterstitialDummy();
      CurrentInterstitial.IsInterstitialCanLaunch = () => true;
      
#if !WITHOUT_BANNER
      Banner = new BannerDummy();
#endif
      Rewarded = new RewardedDummy();
      
      onInitialize?.Invoke();
      Debug.LogWarning("[IAds] You are using Dummy class now");
    }

    public Placements GetPlacements() => 
      new Placements();

    public bool IsNonPersonalized() => 
      false;

    public void SetNonPersonalized(bool value) => 
      Debug.LogWarning("[IAds] You are using Dummy class now");

    public void DisableAds() => 
      Debug.LogWarning("[IAds] You are using Dummy class now");

    public void EnableAds(bool isSplash) => 
      Debug.LogWarning("[IAds] You are using Dummy class now");
    
#if ENABLED_ADMOB
    public void AddListenerOnPaidEvent(UnityAction<AdValueEventArgs, string, string> action)
    {
      Debug.LogWarning("[IAds] You are using Dummy class now");
    }

    public void RemoveListenerOnPaidEvent(UnityAction<AdValueEventArgs, string, string> action)
    {
      Debug.LogWarning("[IAds] You are using Dummy class now");
    }
#endif
  }
}