#if !WITHOUT_BANNER
using System;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using SableGamesAds.Scripts.Common;
using SableGamesAds.Scripts.Interfaces;
using SableGamesAnalytics.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;
#if ENABLED_ADMOB
#endif

namespace SableGamesAds.Scripts.Blocks
{
  public class Banner : IBanner
  {
    private const float DefaultDenominator = 1000000;

    private readonly string _bannerID;
#if ENABLED_ADMOB
    private BannerView _banner;
    
    UnityAction<AdValueEventArgs, string, string> _onPaidEvent;
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
      
      _banner?.Destroy();
      
      _banner = new BannerView(_bannerID, AdSize.Banner, AdPosition.Bottom);
      
      _banner.OnAdLoaded += Banner_OnAdLoaded;
      _banner.OnAdFailedToLoad += Banner_OnAdFailedToLoad;
      _banner.OnPaidEvent += Banner_AdOnPaidEvent;

      _banner.LoadAd(GetRequest());
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
    private AdRequest GetRequest()
    {
      var adRequest = new AdRequest.Builder();
      
      adRequest.AddExtra("npa", PlayerPrefs.GetString("PersonilizedAds", "0"));

      return adRequest.Build();
    }
#endif

#if ENABLED_ADMOB
    private void Banner_AdOnPaidEvent(object sender, AdValueEventArgs args) => 
     OnPaid(args);

    private void OnPaid(AdValueEventArgs args)
    {
      if(args.AdValue.Value == 0)
        return;

      _analytics.Send("ad_ban_paid", new Dictionary<string, object>
      {
        {"placement_id", _bannerID},
        {"price", args.AdValue.Value/DefaultDenominator}
      });
      
      TrackOnPaidEvent(args, _bannerID, "banner");
    }

    public void AddListenerOnPaidEvent(UnityAction<AdValueEventArgs, string, string> action) => 
      _onPaidEvent += action;

    public void RemoveListenerOnPaidEvent(UnityAction<AdValueEventArgs, string, string> action) => 
      _onPaidEvent -= action;
#endif

    private void Banner_OnAdLoaded(object sender, EventArgs e) => 
      UnityThread.Enqueue(OnLoad);

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
    private void TrackOnPaidEvent(AdValueEventArgs args, string placement, string unit) =>
      _onPaidEvent?.Invoke(args, placement, unit);
    
    private void Banner_OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e) => 
      UnityThread.Enqueue(OnFail);

    private void OnFail()
    {
      _heightInPixels = 0;
      _isLoaded = false;
      _onBannerShow?.Invoke(_isLoaded);
    }
#endif
  }
}
#endif