using System;

namespace _Project.Scripts.Features.DisableAds
{
  public interface IDisableAdsViewLogic
  {
    void Construct(string price, Action onShowAds, Action onClose, Action onContinue, Action onPurchase);
    void UpdateAdsWatchedProgress(int progress, int max);
    void UpdateDisableAdsTimer(int sec);
    void OpenWindow();
    void CloseWindow();
    void ShowAdsOfferDisabled();
    void ShowAdsOfferEnabled();
    void ShowInfoPanelBefore();
    void ShowInfoPanelAfter();
  }
}