using System;
using UnityEngine.Events;

namespace Plugins.SableGamesAds.Scripts.Interfaces
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
  }
}