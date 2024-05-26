using GoogleMobileAds.Api;
using SableGamesAds.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;
#if ENABLED_ADMOB
#endif

namespace SableGamesAds.Scripts.Dummy
{
  public class RewardedDummy : IRewarded
  {
    public void RequestRewardedVideo() => 
      Debug.LogWarning("[IAds] You are using Dummy class now");

    public bool RewardIsLoaded() => 
      true;

    public void ShowReward(string rewardInfo, UnityAction reward, UnityAction onNotLoaded = null)
    {
      reward?.Invoke();
      Debug.LogWarning("[IAds] You are using Dummy class now");
    }
    
#if ENABLED_ADMOB
    public void AddListenerOnPaidEvent(UnityAction<AdValueEventArgs, string, string> action) => 
      Debug.LogWarning("[IAds] You are using Dummy class now");

    public void RemoveListenerOnPaidEvent(UnityAction<AdValueEventArgs, string, string> action) =>
      Debug.LogWarning("[IAds] You are using Dummy class now");
#endif
  }
}