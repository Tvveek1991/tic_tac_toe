#if ENABLED_ADMOB
#endif
using System;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using SableGamesAds.Scripts.Common;
using SableGamesAds.Scripts.Interfaces;
using SableGamesAnalytics.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace SableGamesAds.Scripts.Blocks
{
  public class Rewarded : IRewarded
  {
    private const float DefaultDenominator = 1000000;
    private const string RewardedCounterKey = "RewardedCounter";

    private UnityAction _completeReward;
#if ENABLED_ADMOB
    private RewardedAd _rewardBasedVideo;

    UnityAction<AdValueEventArgs, string, string> _onPaidEvent;
#endif
    private readonly string _videoID;

    private UnityAction _reward;

    private readonly IAnalytics _analytics;
    private string _rewardInfo;

    private LoadAdError _adError;

    private int RewardedCounter
    {
      get => PlayerPrefs.GetInt(RewardedCounterKey, 0);
      set => PlayerPrefs.SetInt(RewardedCounterKey, value);
    }

    public Rewarded(string videoID, IAnalytics analytics)
    {
      _analytics = analytics;
      _videoID = videoID;
    }

    public void RequestRewardedVideo()
    {
#if ENABLED_ADMOB
      if (_rewardBasedVideo == null)
      {
        _rewardBasedVideo = new RewardedAd(_videoID);

        _rewardBasedVideo.OnAdFailedToLoad += (sender, args) => { _adError = args.LoadAdError; };
        _rewardBasedVideo.OnUserEarnedReward += HandleRewardBasedVideoRewarded;
        _rewardBasedVideo.OnAdClosed += HandleRewardClosed;
        _rewardBasedVideo.OnPaidEvent += HandleRewardOnPaidEvent;
      }

      _rewardBasedVideo.LoadAd(GetRequestRewarded());
#endif
    }

    public bool RewardIsLoaded()
    {
#if ENABLED_ADMOB
      return _rewardBasedVideo.IsLoaded();
#else
      return false;
#endif
    }

#if ENABLED_ADMOB
    private void HandleRewardOnPaidEvent(object sender, AdValueEventArgs args) =>
      OnPaid(args);

    private void OnPaid(AdValueEventArgs args)
    {
      if (args.AdValue.Value == 0)
        return;

      _analytics.Send("ad_rew_paid", new Dictionary<string, object>
      {
        { "placement_id", _videoID },
        { "reward", _rewardInfo },
        { "price", args.AdValue.Value / DefaultDenominator }
      });

      _analytics.Send("ad_rew_show", new Dictionary<string, object>
      {
        { "reward", _rewardInfo },
        { "placement_id", _videoID }
      });

      //TrackOnPaidEvent(args, _videoID, "rewarded");
    }

    public void AddListenerOnPaidEvent(UnityAction<AdValueEventArgs, string, string> action) =>
      _onPaidEvent += action;

    public void RemoveListenerOnPaidEvent(UnityAction<AdValueEventArgs, string, string> action) =>
      _onPaidEvent -= action;
#endif

    public void ShowReward(string rewardInfo, UnityAction reward, UnityAction onNotLoaded = null)
    {
      _reward = reward;
      _rewardInfo = rewardInfo;

#if ENABLED_ADMOB

      if (_rewardBasedVideo.IsLoaded())
      {
        _rewardBasedVideo.Show();
        _analytics.Send("ad_rew_start", new Dictionary<string, object>
        {
          { "reward", rewardInfo },
          { "placement_id", _videoID }
        });
      }
      else
      {
        var parameters = new Dictionary<string, object>
        {
          { "reward", rewardInfo },
          { "placement_id", _videoID }
        };
        
        if (_adError != null)
        {
          parameters.Add("code", _adError.GetCode());
          parameters.Add("message", _adError.GetMessage());
        }

        _analytics.Send("ad_rew_fail", parameters);

        onNotLoaded?.Invoke();
      }
#else
      RewardToUser();
#endif
    }

#if ENABLED_ADMOB
    private AdRequest GetRequestRewarded()
    {
      var adRequest = new AdRequest.Builder();
      adRequest.AddExtra("npa", PlayerPrefs.GetString("PersonilizedAds", "0"));

      return adRequest.Build();
    }
#endif

    private void HandleRewardClosed(object sender, EventArgs e) =>
      UnityThread.Enqueue(RequestRewardedVideo);

#if ENABLED_ADMOB
    private void TrackOnPaidEvent(AdValueEventArgs args, string placement, string unit) =>
      _onPaidEvent?.Invoke(args, placement, unit);

    private void HandleRewardBasedVideoRewarded(object sender, Reward args) =>
      UnityThread.Enqueue(RewardToUser);
#endif

    private void RewardToUser()
    {
      RewardedCounter++;
      _reward?.Invoke();
    }
  }
}