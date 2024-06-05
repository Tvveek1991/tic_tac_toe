using System;
using GoogleMobileAds.Api;
using UnityEngine.Events;
#if ENABLED_ADMOB

#endif

namespace SableGamesAds.Scripts.Interfaces
{
  public interface IBanner
  {
    void RequestBanner();
    void ShowBanner();
    void HideBanner();
    float GetBannerHeight();
    bool IsLoaded();
    void AddBannerShowListener(UnityAction<bool> onShow);
    void RemoveBannerShowListener(UnityAction<bool> onShow);    
    void SetEnabled(bool value);
    event Action OnBannerLoad;
        
#if ENABLED_ADMOB
    void AddListenerOnPaidEvent(UnityAction<AdValueEventArgs, string, string> action);
    void RemoveListenerOnPaidEvent(UnityAction<AdValueEventArgs, string, string> action);
#endif
  }
}