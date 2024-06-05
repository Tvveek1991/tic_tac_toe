using System;
using System.Globalization;
using SableGamesAds.Scripts.Data;
using SableGamesAds.Scripts.Interfaces;
using SableGamesAds.Scripts.Mono;
using UnityEngine;

namespace SableGamesAds.Scripts.Additional
{
  public class Limitations
  {
    private const string InterstitialTimer = "Interstitial_Timer";

    private int _clickCount;
    
    private readonly SableGamesAdsSettings _adsSettings;

    public Limitations(SableGamesAdsSettings adsSettings, params IInterstitialBase[] interstitialBase)
    {
      _adsSettings = adsSettings;
      
      foreach (IInterstitialBase baseInterstitial in interstitialBase)
      {
        baseInterstitial.IsInterstitialCanLaunch = IsAvailable;
        baseInterstitial.InterstitialLaunched = Launch;
      }

      var handler = new GameObject("Interstitial Limitations Handler")
        .AddComponent<InterstitialLimitationsHandler>();

      handler.OnLateUpdate = LateUpdate;
    }
    
    private bool IsAvailable()
    {
      if (!_adsSettings.EnableLimitations)
        return true;
      
      bool isAvailable;

      if (_adsSettings.TimeLimit == 0)
      {
        var enoughClicks = CheckEnoughClicks();
        isAvailable = enoughClicks;
      }
      else
      {
        var enoughClicks = CheckEnoughClicks();
        var timeIsReady = CheckTimeIsReady();
        isAvailable = enoughClicks && timeIsReady;
      }

      return isAvailable;
    }

    private void Launch()
    {
      PlayerPrefs.SetString(InterstitialTimer, DateTime.Now.ToString(CultureInfo.InvariantCulture));
      _clickCount = 0;
    }

    private void LateUpdate()
    {
      if (Input.GetKeyDown(KeyCode.Mouse0))
        _clickCount++;
    }

    private bool CheckEnoughClicks()
    {
      var enoughClicks = _clickCount >= _adsSettings.ClicksLimit;
      return enoughClicks;
    }

    private bool CheckTimeIsReady()
    {
      var defaultValue = DateTime.MinValue.ToString(CultureInfo.InvariantCulture);
      var timerValue = PlayerPrefs.GetString(InterstitialTimer, defaultValue);
      var dateSaved = DateTime.Parse(timerValue, CultureInfo.InvariantCulture);
      var timeIsReady = (DateTime.Now - dateSaved).TotalSeconds >= _adsSettings.TimeLimit;
      return timeIsReady;
    }
  }
}