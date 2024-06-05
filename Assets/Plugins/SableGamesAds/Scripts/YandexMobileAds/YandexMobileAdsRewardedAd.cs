using System.Collections.Generic;
using Plugins.SableGamesAds.Scripts.Interfaces;
using SableGamesAds.Scripts.Common;
using SableGamesAnalytics.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;
#if ENABLED_YANDEX
using AdRequest = YandexMobileAds.Base.AdRequest;
using RewardedAd = YandexMobileAds.RewardedAd;
#endif

namespace Plugins.SableGamesAds.Scripts.YandexMobileAds
{
  public class YandexMobileAdsRewardedAd : IRewarded
  {
    private const string RewardedCounterKey = "RewardedCounter";
#if ENABLED_YANDEX
    private readonly RewardedAd _rewardedAd;
#endif
    private readonly IAnalytics _analytics;

    private readonly string _placementId;
    private string _rewardInfo;

    private UnityAction _reward;
    private UnityAction _completeReward;

    private string _failMessage;

    private int RewardedCounter
    {
      get => PlayerPrefs.GetInt(RewardedCounterKey, 0);
      set => PlayerPrefs.SetInt(RewardedCounterKey, value);
    }

    public YandexMobileAdsRewardedAd(string placementId, IAnalytics analytics)
    {
      _analytics = analytics;
      _placementId = placementId;

#if ENABLED_YANDEX
      _rewardedAd = new RewardedAd(_placementId);

      _rewardedAd.OnRewardedAdFailedToLoad += (_, args) => { _failMessage = args.Message; };
      _rewardedAd.OnRewardedAdShown += (_, _) => { };
      _rewardedAd.OnRewardedAdDismissed += (_, _) => { HandleRewardClosed(); };
      _rewardedAd.OnRewarded += (_, _) => { HandleRewardBasedVideoRewarded(); };
#endif
    }

    public void RequestRewardedVideo()
    {
#if ENABLED_YANDEX
      _rewardedAd.LoadAd(new AdRequest.Builder().Build());
#endif
    }

    public bool RewardIsLoaded()
    {
#if ENABLED_YANDEX
      return _rewardedAd.IsLoaded();
#else
      return true;
#endif
    }

    public void ShowReward(string rewardInfo, UnityAction reward, UnityAction onNotLoaded = null)
    {
      _reward = reward;
      _rewardInfo = rewardInfo;

      if (RewardIsLoaded())
      {
#if ENABLED_YANDEX
        _rewardedAd.Show();
#endif
        _analytics.Send("ad_rew_start", new Dictionary<string, object>
        {
          { "reward", rewardInfo },
          { "placement_id", _placementId }
        });
      }
      else
      {
        var parameters = new Dictionary<string, object>
        {
          { "reward", rewardInfo },
          { "placement_id", _placementId },
        };

        if (_failMessage != null)
          parameters.Add("message", _failMessage);

        _analytics.Send("ad_rew_fail", parameters);

        onNotLoaded?.Invoke();
      }
    }

    private void HandleRewardBasedVideoRewarded() =>
      UnityThread.Enqueue(RewardToUser);

    private void RewardToUser()
    {
      _analytics.Send("ad_rew_show", new Dictionary<string, object>
      {
        { "reward", _rewardInfo },
        { "placement_id", _placementId }
      });

      RewardedCounter++;

      _reward?.Invoke();

      RequestRewardedVideo();
    }

    private void HandleRewardClosed() =>
      UnityThread.Enqueue(RequestRewardedVideo);
  }
}