using System.Collections.Generic;
using GoogleMobileAds.Api;
using Plugins.SableGamesAds.Scripts.Interfaces;
using SableGamesAnalytics.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Plugins.SableGamesAds.Scripts.Blocks
{
  public class Rewarded : IRewarded
  {
    private const float DefaultDenominator = 1000000;

    private UnityAction _completeReward;
#if ENABLED_ADMOB
    private RewardedAd _rewardBasedVideo;
#endif
    private readonly string _videoID;

    private UnityAction _reward;

    private readonly IAnalytics _analytics;
    private string _rewardInfo;

    private AdError _adError;
    
    public Rewarded(string videoID, IAnalytics analytics)
    {
      _analytics = analytics;
      _videoID = videoID;
    }

    public void RequestRewardedVideo()
    {
#if ENABLED_ADMOB
      // Clean up the old ad before loading a new one.
      if (_rewardBasedVideo != null)
      {
        _rewardBasedVideo.Destroy();
        _rewardBasedVideo = null;
      }

      var adRequest = new AdRequest();

      // send the request to load the ad.
      RewardedAd.Load(_videoID, adRequest,
        (ad, error) =>
        {
          if (error != null || ad == null)
          {
            Debug.LogError("Rewarded ad failed to load an ad " +
                           "with error : " + error);
            return;
          }

          Debug.Log("Rewarded ad loaded with response : "
                    + ad.GetResponseInfo());

          _rewardBasedVideo = ad;
          
          RegisterEventHandlers(_rewardBasedVideo);
          RegisterReloadHandler(_rewardBasedVideo);
        });
#endif
    }

    public bool RewardIsLoaded()
    {
#if ENABLED_ADMOB
      return _rewardBasedVideo != null && _rewardBasedVideo.CanShowAd();
#else
      return false;
#endif
    }

#if ENABLED_ADMOB
    public void ShowReward(string rewardInfo, UnityAction reward, UnityAction onNotLoaded = null)
    {
      _reward = reward;
      _rewardInfo = rewardInfo;

#if ENABLED_ADMOB
      if (_rewardBasedVideo != null && _rewardBasedVideo.CanShowAd())
      {
        _rewardBasedVideo.Show(_ =>
        {
          RewardToUser();
        });
          
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

    private void OnPaid(AdValue adValue)
    {
      if (adValue.Value == 0)
        return;

      _analytics.Send("ad_rew_paid", new Dictionary<string, object>
      {
        { "placement_id", _videoID },
        { "reward", _rewardInfo },
        { "price", adValue.Value / DefaultDenominator }
      });

      _analytics.Send("ad_rew_show", new Dictionary<string, object>
      {
        { "reward", _rewardInfo },
        { "placement_id", _videoID }
      });
    }
#endif


#if ENABLED_ADMOB
    private void RegisterEventHandlers(RewardedAd ad)
    {
      ad.OnAdPaid += OnPaid;
      ad.OnAdFullScreenContentFailed += error =>
      {
        _adError = error;
        Debug.LogError("Rewarded ad failed to open full screen content " +
                       "with error : " + error);
      };
    }
    
    private void RegisterReloadHandler(RewardedAd ad)
    {
      // Raised when the ad closed full screen content.
      ad.OnAdFullScreenContentClosed += RequestRewardedVideo;
      // Raised when the ad failed to open full screen content.
      ad.OnAdFullScreenContentFailed += _ => RequestRewardedVideo();
    }
#endif
    
    private void RewardToUser() => 
      _reward?.Invoke();
  }
}