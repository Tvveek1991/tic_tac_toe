using System;
using Plugins.SableGamesAds.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace SableGamesAds.Scripts.Dummy
{
  public class BannerDummy : IBanner
  {
    public event Action OnBannerLoad;

    public void RequestBanner()
    {
      Debug.LogWarning("[IAds] You are using Dummy class now");
      OnBannerLoad?.Invoke();
    }

    public void ShowBanner() =>
      Debug.LogWarning("[IAds] You are using Dummy class now");

    public void HideBanner() =>
      Debug.LogWarning("[IAds] You are using Dummy class now");

    public float GetBannerHeight() => 0;

    public bool IsLoaded() =>
      true;

    public void AddBannerShowListener(UnityAction<bool> onShow) =>
      Debug.LogWarning("[IAds] You are using Dummy class now");

    public void RemoveBannerShowListener(UnityAction<bool> onShow) =>
      Debug.LogWarning("[IAds] You are using Dummy class now");

    public void SetEnabled(bool value) =>
      Debug.LogWarning("[IAds] You are using Dummy class now");
  }
}