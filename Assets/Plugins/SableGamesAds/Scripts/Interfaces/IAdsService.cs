using System;
using GoogleMobileAds.Api;
using SableGamesAds.Scripts.Common;
using SableGamesAds.Scripts.Data;
using SableGamesRemote.Scripts.Data;
using UnityEngine.Events;
#if ENABLED_ADMOB

#endif

namespace SableGamesAds.Scripts.Interfaces
{
  public interface IAdsService : IService
  {
    void Initialize(PlacementType placementType, Action onInitialize, bool isSplash = false);
    Placements GetPlacements();
    bool IsNonPersonalized();
    void SetNonPersonalized(bool value);
    void DisableAds();
#if !WITHOUT_BANNER
    IBanner Banner { get; set; }
#endif
    IRewarded Rewarded { get; set; }
    IInterstitialBase CurrentInterstitial { get; set; }
    bool AdsEnabled { get; set; }
    void EnableAds(bool isSplash);
    
    
#if ENABLED_ADMOB
    void AddListenerOnPaidEvent(UnityAction<AdValueEventArgs, string, string> action);
    void RemoveListenerOnPaidEvent(UnityAction<AdValueEventArgs, string, string> action);
#endif
  }
}