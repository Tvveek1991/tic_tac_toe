using System;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using Plugins.SableGamesAds.Scripts.Interfaces;
using SableGamesAnalytics.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Plugins.SableGamesAds.Scripts.Blocks
{
  public class Interstitial : IInterstitial
  {
    private const float DefaultDenominator = 1000000;

    private readonly string _interstitialSplashID;
    private readonly string _interstitialID;
    private string _currentId;

    private readonly IAnalytics _analytics;
#if ENABLED_ADMOB
    private InterstitialAd _interstitialAd;

    UnityAction<AdValueEventArgs, string, string> _onPaidEvent;
#endif

    UnityAction _onEndInterstitial;
    
    private string _targetScreen;
    private Action _onInterstitialLoaded;

    public InterstitialLaunchCondition IsInterstitialCanLaunch { get; set; }

    public Action InterstitialLaunched { get; set; }

    private bool _isEnabled = true;
    private bool _isInitialized;

    private AdError _adError;
    
    public Interstitial(string interstitialSplashID, string interstitialID, IAnalytics analytics, Action onInterstitialLoaded)
    {
      _onInterstitialLoaded = onInterstitialLoaded;

      _analytics = analytics;
      _interstitialID = interstitialID;
      _interstitialSplashID = interstitialSplashID;
    }

    public void SetEnabled(bool value) =>
      _isEnabled = value;

    public bool InterstitialIsLoaded()
    {
#if ENABLED_ADMOB
      return _interstitialAd != null && _interstitialAd.CanShowAd();
#else
      return false;
#endif
    }

    public void ShowInterstitial(string targetScreen, UnityAction onEnd, UnityAction onFail, bool skipRestrictions = false)
    {
      if (!_isEnabled)
      {
        onFail?.Invoke();
        return;
      }

      _onEndInterstitial = onEnd;

      if (!skipRestrictions)
      {
        if (IsInterstitialCanLaunch != null)
          if (!IsInterstitialCanLaunch.Invoke())
          {
            _onEndInterstitial?.Invoke();
            return;
          }
      }

      InnerInterstitialShow(targetScreen, onFail);
    }

    public void RequestInterstitial(bool isSplash)
    {
#if ENABLED_ADMOB
      if (!_isEnabled)
      {
        LoadCallback();
        return;
      }

      _currentId = isSplash ? _interstitialSplashID : _interstitialID;

      if (_interstitialAd != null)
      {
        _interstitialAd.Destroy();
        _interstitialAd = null;
      }

      Debug.Log("Loading the interstitial ad.");

      var adRequest = new AdRequest();

      // send the request to load the ad.
      InterstitialAd.Load(_currentId, adRequest,
        (ad, error) =>
        {
          // if error is not null, the load request failed.
          if (error != null || ad == null)
          {
            Debug.LogError("interstitial ad failed to load an ad " +
                           "with error : " + error);
            
            LoadCallback();
            
            return;
          }

          Debug.Log("Interstitial ad loaded with response : "
                    + ad.GetResponseInfo());

          _interstitialAd = ad;
          
          RegisterEventHandlers(_interstitialAd);
          RegisterReloadHandler(_interstitialAd);
          
          LoadCallback();
        });
#endif
    }

#if ENABLED_ADMOB

    private void OnPaid(AdValue adValue)
    {
      if (adValue.Value == 0)
        return;

      _analytics.Send("ad_int_paid", new Dictionary<string, object>
      {
        { "placement_id", _currentId },
        { "to_screen", _targetScreen },
        { "price", adValue.Value / DefaultDenominator }
      });

      _analytics.Send("ad_int_show", new Dictionary<string, object>
      {
        { "to_screen", _targetScreen },
        { "placement_id", _currentId }
      });
    }

#endif

    private void LoadCallback()
    {
      Debug.Log("[IAnalytics] InterstitialAdOnOnAdLoaded");

      if (_isInitialized)
        return;

      _isInitialized = true;

      _onInterstitialLoaded?.Invoke();
      _onInterstitialLoaded = null;
    }

    private void InnerInterstitialShow(string targetScreen, UnityAction onFail)
    {
      _targetScreen = targetScreen;

#if ENABLED_ADMOB

      _analytics.Send("ad_int_trig", new Dictionary<string, object>
      {
        { "to_screen", targetScreen },
        { "placement_id", _currentId }
      });

      if (_interstitialAd != null && _interstitialAd.CanShowAd())
      {
        InterstitialLaunched?.Invoke();
        _interstitialAd.Show();
        _analytics.Send("ad_int_start", new Dictionary<string, object>
        {
          { "to_screen", targetScreen },
          { "placement_id", _currentId }
        });
      }
      else
      {
        var parameters = new Dictionary<string, object>
        {
          { "to_screen", targetScreen },
          { "placement_id", _currentId }
        };

        if (_adError != null)
        {
          parameters.Add("code", _adError.GetCode());
          parameters.Add("message", _adError.GetMessage());
        }

        _analytics.Send("ad_int_fail", parameters);

        onFail?.Invoke();
      }
#else
      onFail?.Invoke();
#endif
    }

    private void OnInterstitialClosed()
    {
      RequestInterstitial(false);
      _onEndInterstitial?.Invoke();
    }

#if ENABLED_ADMOB
    private void RegisterEventHandlers(InterstitialAd ad)
    {
      ad.OnAdPaid += OnPaid;
      ad.OnAdFullScreenContentClosed += OnInterstitialClosed;
      ad.OnAdFullScreenContentFailed += error =>
      {
        _adError = error;
        Debug.LogError("Rewarded ad failed to open full screen content " +
                       "with error : " + error);
      };
    }

    private void RegisterReloadHandler(InterstitialAd ad)
    {
      // Raised when the ad closed full screen content.
      ad.OnAdFullScreenContentClosed += () => RequestInterstitial(false);

      // Raised when the ad failed to open full screen content.
      ad.OnAdFullScreenContentFailed += _ => RequestInterstitial(false);
    }
#endif
  }
}