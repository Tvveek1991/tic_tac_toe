using System;
using System.Collections.Generic;
using Plugins.SableGamesAds.Scripts.Interfaces;
using SableGamesAds.Scripts.Common;
using SableGamesAnalytics.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using YandexMobileAds;
using YandexMobileAds.Base;
#if ENABLED_YANDEX
using AdRequest = YandexMobileAds.Base.AdRequest;

#endif

namespace SableGamesAds.Scripts.YandexMobileAds
{
  public class YandexMobileAdsInterstitialAd : IInterstitial
  {
    private const string InterstitialCounterKey = "InterstitialCounter";
#if ENABLED_YANDEX
    private readonly Interstitial _interstitialAd;
#endif
    private UnityAction _onEndInterstitial;
    private Action _onInterstitialLoaded;

    private bool _isEnabled;

    private readonly IAnalytics _analytics;

    private string _targetScreen;
    private readonly string _currentId;

    private bool _isInitialized;

    private int InterstitialCounter
    {
      get => PlayerPrefs.GetInt(InterstitialCounterKey, 0);
      set => PlayerPrefs.SetInt(InterstitialCounterKey, value);
    }

    public InterstitialLaunchCondition IsInterstitialCanLaunch { get; set; }
    public Action InterstitialLaunched { get; set; }

    private string _failMessage;

    public YandexMobileAdsInterstitialAd(IAnalytics analytics, string placementId, Action onInterstitialLoaded)
    {
      _currentId = placementId;
      _onInterstitialLoaded = onInterstitialLoaded;
      _analytics = analytics;

#if ENABLED_YANDEX
      _interstitialAd = new Interstitial(placementId);

      _interstitialAd.OnInterstitialLoaded += (_, _) => InterstitialLoaded();
      _interstitialAd.OnInterstitialFailedToLoad += (_, args) => InterstitialFailedToLoad(args);
      _interstitialAd.OnInterstitialDismissed += (_, _) => InterstitialClosed();
#endif
    }

    public bool InterstitialIsLoaded()
    {
#if ENABLED_YANDEX
      return _interstitialAd.IsLoaded();
#else
      return true;
#endif
    }

    public void RequestInterstitial(bool isSplash)
    {
#if ENABLED_YANDEX
      _interstitialAd.LoadAd(new AdRequest.Builder().Build());
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

    private void InnerInterstitialShow(string targetScreen, UnityAction onFail)
    {
      _targetScreen = targetScreen;

      _analytics.Send("ad_int_trig", new Dictionary<string, object>
      {
        { "to_screen", targetScreen },
        { "placement_id", _currentId }
      });

      if (InterstitialIsLoaded())
      {
        InterstitialLaunched?.Invoke();
#if ENABLED_YANDEX
        _interstitialAd.Show();
#endif
        _analytics.Send("ad_int_start", new Dictionary<string, object>
        {
          { "to_screen", targetScreen },
          { "placement_id", _currentId }
        });

        InterstitialCounter++;
      }
      else
      {
        var parameters = new Dictionary<string, object>
        {
          { "to_screen", targetScreen },
          { "placement_id", _currentId }
        };
        
        if(_failMessage != null)
          parameters.Add("message", _failMessage);
        
        _analytics.Send("ad_int_fail", parameters);
        onFail?.Invoke();
      }
    }

    public void SetEnabled(bool value) =>
      _isEnabled = value;

    private void InterstitialFailedToLoad(AdFailureEventArgs adFailureEventArgs)
    {
      _failMessage = adFailureEventArgs.Message;
      UnityThread.Enqueue(LoadCallback);
    }

    private void LoadCallback()
    {
      if (_isInitialized)
        return;

      _isInitialized = true;

      _onInterstitialLoaded?.Invoke();
      _onInterstitialLoaded = null;
    }

    private void InterstitialLoaded() =>
      UnityThread.Enqueue(LoadCallback);

    private void InterstitialClosed() =>
      UnityThread.Enqueue(OnInterstitialClosed);

    private void OnInterstitialClosed()
    {
      RequestInterstitial(false);

      _analytics.Send("ad_int_show", new Dictionary<string, object>
      {
        { "to_screen", _targetScreen },
        { "placement_id", _currentId }
      });

      _onEndInterstitial?.Invoke();
    }
  }
}