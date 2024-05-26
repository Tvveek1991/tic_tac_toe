#if !WITHOUT_BANNER
#if ENABLED_YANDEX
using System;
using System.Collections.Generic;
using YandexMobileAds;
using AdPosition = YandexMobileAds.Base.AdPosition;
using AdRequest = YandexMobileAds.Base.AdRequest;
using AdSize = YandexMobileAds.Base.AdSize;
#endif
#if ENABLED_ADMOB
using GoogleMobileAds.Api;
using SableGamesAds.Scripts.Common;
using SableGamesAds.Scripts.Interfaces;
using SableGamesAnalytics.Scripts.Interfaces;
using UnityEngine.Events;

#endif

namespace SableGamesAds.Scripts.YandexMobileAds
{
  public class YandexMobileAdsBanner : IBanner
  {
#if ENABLED_YANDEX
    private readonly Banner _banner;
#endif
    
    private bool _isLoaded;
    private bool _isEnabled;

    private event UnityAction<bool> _onShow;
    public event Action OnBannerLoad;

    public YandexMobileAdsBanner(IAnalytics analytics, string placementId)
    {
      string bannerId = placementId;
#if ENABLED_YANDEX
      _banner = new Banner(bannerId, AdSize.BANNER_320x50, AdPosition.BottomCenter);

      _banner.OnAdLoaded += (sender, args) => 
        UnityThread.Enqueue(() => OnLoad(analytics, bannerId));
      
      _banner.OnAdFailedToLoad += (sender, args) => _isLoaded = false;
#endif
    }

    private void OnLoad(IAnalytics analytics, string bannerId)
    {
      _isLoaded = true;
      OnBannerLoad?.Invoke();

      analytics.Send("ad_ban_start", new Dictionary<string, object>
      {
        { "placement_id", bannerId }
      });
    }

    public void RequestBanner()
    {   
      if(!_isEnabled)
        return;
#if ENABLED_YANDEX
      _banner.LoadAd(new AdRequest.Builder().Build());
#endif
    }

    public void ShowBanner()
    {
      if(!_isEnabled)
        return;
#if ENABLED_YANDEX
      _banner.Show();
#endif
      _onShow?.Invoke(true);
    }

    public void HideBanner()
    {
#if ENABLED_YANDEX
      _banner.Hide();
#endif
      _onShow?.Invoke(false);
    }

    public float GetBannerHeight() => 
      100;

    public bool IsLoaded() => 
      _isLoaded;

    public void AddBannerShowListener(UnityAction<bool> onShow) => 
      _onShow += onShow;

    public void RemoveBannerShowListener(UnityAction<bool> onShow) => 
      _onShow -= onShow;

    public void SetEnabled(bool value)
    {
      _isEnabled = value;
      HideBanner();
    }
    
#if ENABLED_ADMOB
    public void AddListenerOnPaidEvent(UnityAction<AdValueEventArgs, string, string> action) {}
    public void RemoveListenerOnPaidEvent(UnityAction<AdValueEventArgs, string, string> action){}
#endif
  }
}
#endif