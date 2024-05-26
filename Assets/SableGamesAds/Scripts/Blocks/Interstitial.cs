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
  public class Interstitial : IInterstitial
  {
    private const float DefaultDenominator = 1000000;
    private const string InterstitialCounterKey = "InterstitialCounter";

    private readonly string _interstitialSplashID;
    private readonly string _interstitialID;
    private string _currentId;

    private readonly IAnalytics _analytics;
#if ENABLED_ADMOB
    private InterstitialAd _interstitialAd;
    
    UnityAction<AdValueEventArgs, string, string> _onPaidEvent;
#endif

    UnityAction _onEndInterstitial;

    private int _requestCounter;

    private string _targetScreen;
    private Action _onInterstitialLoaded;

    public InterstitialLaunchCondition IsInterstitialCanLaunch { get; set; }

    public Action InterstitialLaunched { get; set; }

    private bool _isEnabled = true;

    private bool _isInitialized = false;
    
    private LoadAdError _adError;

    private int InterstitialCounter
    {
      get => PlayerPrefs.GetInt(InterstitialCounterKey, 0);
      set => PlayerPrefs.SetInt(InterstitialCounterKey, value);
    }

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
      return _interstitialAd.IsLoaded();
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

      if (_interstitialAd == null)
      {
        _interstitialAd = new InterstitialAd(_currentId);
        _interstitialAd.OnAdClosed += InterstitialOnAdClosed;
        _interstitialAd.OnAdLoaded += InterstitialOnAdLoaded;
        _interstitialAd.OnAdFailedToLoad += InterstitialOnAdFailedToLoad;
        _interstitialAd.OnPaidEvent += InterstitialAdOnPaidEvent;
      }

      _interstitialAd.LoadAd(GetRequestInterstitial());

      _requestCounter++;
#endif
    }

#if ENABLED_ADMOB
    private void InterstitialOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
      _adError = e.LoadAdError;
      UnityThread.Enqueue(LoadCallback);
    }

    private void InterstitialAdOnPaidEvent(object sender, AdValueEventArgs args) => 
      OnPaid(args);

    private void OnPaid(AdValueEventArgs args)
    {
      if(args.AdValue.Value == 0)
        return;

      _analytics.Send("ad_int_paid", new Dictionary<string, object>
      {
        {"placement_id", _currentId},
        {"to_screen", _targetScreen},
        {"price", args.AdValue.Value / DefaultDenominator}
      });

      _analytics.Send("ad_int_show", new Dictionary<string, object>
      {
        {"to_screen", _targetScreen},
        {"placement_id", _currentId}
      });

      //TrackOnPaidEvent(args, _currentId, "interstitial");
    }

#endif

    private void LoadCallback()
    {   
      Debug.Log("[IAnalytics] InterstitialAdOnOnAdLoaded");

      if(_isInitialized)
        return;
      
      _isInitialized = true;
      
      _onInterstitialLoaded?.Invoke();
      _onInterstitialLoaded = null;
    }
    
    private void InterstitialOnAdLoaded(object sender, EventArgs e) => 
      UnityThread.Enqueue(LoadCallback);

    private void InnerInterstitialShow(string targetScreen, UnityAction onFail)
    {
      _targetScreen = targetScreen;

#if ENABLED_ADMOB

      _analytics.Send("ad_int_trig", new Dictionary<string, object>
      {
        {"to_screen", targetScreen}, 
        {"placement_id", _currentId}
      });

      if (_interstitialAd.IsLoaded())
      {
        InterstitialLaunched?.Invoke();
        _interstitialAd.Show();
        _analytics.Send("ad_int_start", new Dictionary<string, object>
        {
          {"to_screen", targetScreen}, 
          {"placement_id", _currentId}
        });

        InterstitialCounter++;
      }
      else
      {
        var parameters = new Dictionary<string, object>
        {
          {"to_screen", targetScreen}, 
          {"placement_id", _currentId}
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

    private void InterstitialOnAdClosed(object sender, EventArgs e) => 
      UnityThread.Enqueue(OnInterstitialClosed);

    private void OnInterstitialClosed()
    {    
      RequestInterstitial(false);
      _onEndInterstitial?.Invoke();
    }

#if ENABLED_ADMOB
    private AdRequest GetRequestInterstitial()
    {
      var adRequest = new AdRequest.Builder();
      adRequest.AddExtra("npa", PlayerPrefs.GetString("PersonilizedAds", "0"));

      return adRequest.Build();
    }
    
    private void TrackOnPaidEvent(AdValueEventArgs args, string placement, string unit) =>
      _onPaidEvent?.Invoke(args, placement, unit);

    public void AddListenerOnPaidEvent(UnityAction<AdValueEventArgs, string, string> action) => 
      _onPaidEvent += action;

    public void RemoveListenerOnPaidEvent(UnityAction<AdValueEventArgs, string, string> action) => 
      _onPaidEvent -= action;
#endif
  }
}