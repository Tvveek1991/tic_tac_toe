using Plugins.SableGamesAds.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;

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
  }
}