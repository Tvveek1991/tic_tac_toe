using System;

namespace SableGamesAds.Scripts.Data
{
  [Serializable]
  public struct Placements
  {
#if !WITHOUT_BANNER
    public string Banner;
#endif
    public string Interstitial;
    public string SplashInterstitial;
    public string Rewarded;
  }
}