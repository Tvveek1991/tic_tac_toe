using GoogleMobileAds.Api;
using UnityEngine.Events;
#if ENABLED_ADMOB

#endif

namespace SableGamesAds.Scripts.Interfaces
{
  public interface IRewarded
  {
    void RequestRewardedVideo();
    bool RewardIsLoaded();
    void ShowReward(string rewardInfo, UnityAction reward, UnityAction onNotLoaded = null);    
    
#if ENABLED_ADMOB
    void AddListenerOnPaidEvent(UnityAction<AdValueEventArgs, string, string> action);
    void RemoveListenerOnPaidEvent(UnityAction<AdValueEventArgs, string, string> action);
#endif
  }
}