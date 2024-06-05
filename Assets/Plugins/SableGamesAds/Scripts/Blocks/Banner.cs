#if !WITHOUT_BANNER
using System;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using Plugins.SableGamesAds.Scripts.Interfaces;
using SableGamesAnalytics.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Plugins.SableGamesAds.Scripts.Blocks
{
  public class Banner : IBanner
  {
    private const float DefaultDenominator = 1000000;

    private readonly string _bannerID;
#if ENABLED_ADMOB
    private BannerView _banner;
#endif

    private UnityAction<bool> _onBannerShow;
    private float _heightInPixels;

    private bool _isLoaded;
    private bool _isOneTimeShift;
    private bool _isEnabled;

    private readonly IAnalytics _analytics;
    
    public event Action OnBannerLoad;

    public Banner(string bannerID, IAnalytics analytics)
    {
      _analytics = analytics;
      _bannerID = bannerID;
    }

    public void SetEnabled(bool value) =>
      _isEnabled = value;

    public void RequestBanner()
    {
#if ENABLED_ADMOB
      if(!_isEnabled)
        return;
      
      Debug.Log("Creating banner view");

      if (_banner != null) 
        DestroyAd();

      _banner = new BannerView(_bannerID, AdSize.Banner, AdPosition.Bottom);
      
      var adRequest = new AdRequest();
      _banner.LoadAd(adRequest);
      
      ListenToAdEvents();
#endif
    }

    public void ShowBanner()
    {
#if ENABLED_ADMOB
      if(!_isEnabled)
        return;
      
      _onBannerShow?.Invoke(true);
      _banner?.Show();
#endif
    }

    public void HideBanner()
    {
#if ENABLED_ADMOB
      _onBannerShow?.Invoke(false);
      _banner?.Hide();
#endif
    }

    public float GetBannerHeight() =>
      _heightInPixels;

    public bool IsLoaded() =>
      _isLoaded;

    public void AddBannerShowListener(UnityAction<bool> onShow) =>
      _onBannerShow += onShow;

    public void RemoveBannerShowListener(UnityAction<bool> onShow) =>
      _onBannerShow -= onShow;
    
#if ENABLED_ADMOB
    private void OnPaid(AdValue adValue)
    {
      if(adValue.Value == 0)
        return;

      _analytics.Send("ad_ban_paid", new Dictionary<string, object>
      {
        {"placement_id", _bannerID},
        {"price", adValue.Value/DefaultDenominator}
      });
    }
#endif

    private void OnLoad()
    {
#if ENABLED_ADMOB
      _heightInPixels = _banner.GetHeightInPixels();
      _isLoaded = true;

      if (_isOneTimeShift)
      {
        _onBannerShow?.Invoke(_isLoaded);
        _isOneTimeShift = true;
      }

      _analytics.Send("ad_ban_start", new Dictionary<string, object>
      {
        { "placement_id", _bannerID }
      });
#endif
      OnBannerLoad?.Invoke();
    }

#if ENABLED_ADMOB
    private void OnFail()
    {
      _heightInPixels = 0;
      _isLoaded = false;
      _onBannerShow?.Invoke(_isLoaded);
    }

    private void ListenToAdEvents()
    {
      _banner.OnBannerAdLoaded += OnLoad;
      _banner.OnBannerAdLoadFailed += _ => OnFail();
      _banner.OnAdPaid += OnPaid;
    }
    
    private void DestroyAd()
    {
      if (_banner != null)
      {
        Debug.Log("Destroying banner ad.");
        _banner.Destroy();
        _banner = null;
      }
    }
#endif
  }
}
#endif