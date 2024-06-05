using System;
using SableGamesAds.Scripts.Common;
using SableGamesAds.Scripts.Data;
using SableGamesRemote.Scripts.Data;

namespace Plugins.SableGamesAds.Scripts.Interfaces
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
  }
}