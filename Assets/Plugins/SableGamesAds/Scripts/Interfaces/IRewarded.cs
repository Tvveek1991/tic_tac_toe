using UnityEngine.Events;

namespace Plugins.SableGamesAds.Scripts.Interfaces
{
  public interface IRewarded
  {
    void RequestRewardedVideo();
    bool RewardIsLoaded();
    void ShowReward(string rewardInfo, UnityAction reward, UnityAction onNotLoaded = null);
  }
}