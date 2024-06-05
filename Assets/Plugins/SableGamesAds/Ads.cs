using System;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using Plugins.SableGamesAds.Scripts.Blocks;
using Plugins.SableGamesAds.Scripts.Interfaces;
using Plugins.SableGamesAds.Scripts.YandexMobileAds;
using SableGamesAds.Scripts.Additional;
using SableGamesAds.Scripts.Data;
using SableGamesAds.Scripts.Logger;
using SableGamesAds.Scripts.YandexMobileAds;
using SableGamesAnalytics.Scripts.Interfaces;
using SableGamesRemote.Scripts.Data;
using UnityEngine;

#if ENABLED_ADMOB
#endif

namespace Plugins.SableGamesAds
{
  public class AdsService : IAdsService
  {
    private const string GoogleAdsPersonalizedAds = "GoogleAds.PersonilizedAds";
    private const string AdsShowLogicAds = "AdsShowLogic.Ads";

    private SableGamesAdsSettings _adsSettings;
    private Dictionary<PlacementType, Dictionary<string, string>> _placements;

#if !WITHOUT_BANNER
    private string _bannerID;
#endif
    private string _interstitialID;
    private string _videoID;
    private string _interstitialSplashID;

    private readonly IAnalytics _analytics;
#if !WITHOUT_BANNER
    public IBanner Banner { get; set; }
#endif
    public IInterstitialBase CurrentInterstitial { get; set; }
    public IRewarded Rewarded { get; set; }

    private IInterstitial _interstitial;

    private Action _onInitialize;
    private readonly IAdsLogger _adsLogger;

    private bool _isSng;
    private Location _location;

    public bool AdsEnabled
    {
      get => PlayerPrefs.GetInt(AdsShowLogicAds, 1) == 1;
      set => PlayerPrefs.SetInt(AdsShowLogicAds, value ? 1 : 0);
    }

    public AdsService(IAnalytics analytics, IAdsLogger adsLogger)
    {
      _adsLogger = adsLogger;
      _analytics = analytics;
    }

    public void Initialize(PlacementType placementType, Action onInitialize, bool isSplash = false)
    {
#if !ENABLED_ADMOB
      throw new Exception("Admob for Sable Games package is disabled - add Google Mobile Ads package & include directive ENABLED_ADMOB");
#endif
#if !ENABLED_YANDEX
      throw new Exception("Yandex for Sable Games package is disabled - include directive ENABLED_YANDEX");
#endif
      
      _location = new Location();
      _location.RequestForCode(countryCode =>
      {
        _onInitialize = onInitialize;
        _isSng = ShouldEnableAlternativeAds(countryCode);

        LoadSettings();

        _adsLogger.LogWithoutFormat($"Initialize Ads With {placementType}");
        AdjustPlacements(placementType);

        LoadAdsService();

        InitializeBlocks();
        SetAdsBlocksState();

        CurrentInterstitial = _interstitial;

        if (AdsEnabled)
          new Limitations(_adsSettings, _interstitial);

        CurrentInterstitial = _interstitial;

        RequestAds(isSplash);
      });
    }

    public void DisableAds()
    {
      AdsEnabled = false;
#if !WITHOUT_BANNER
      Banner.HideBanner();
      Banner.SetEnabled(AdsEnabled);
#endif
      _interstitial.SetEnabled(AdsEnabled);
    }

    public void EnableAds(bool isSplash = false)
    {
      AdsEnabled = true;
#if !WITHOUT_BANNER
      Banner.ShowBanner();
      Banner.SetEnabled(AdsEnabled);
#endif
      _interstitial.SetEnabled(AdsEnabled);

      RequestAds(isSplash);
    }

    public Placements GetPlacements()
    {
      return new Placements()
      {
#if !WITHOUT_BANNER
        Banner = _bannerID,
#endif
        Interstitial = _interstitialID,
        SplashInterstitial = _interstitialSplashID,
        Rewarded = _videoID,
      };
    }

    public bool IsNonPersonalized() =>
      PlayerPrefs.GetString(GoogleAdsPersonalizedAds, "0").Equals("1");

    public void SetNonPersonalized(bool value) =>
      PlayerPrefs.SetString(GoogleAdsPersonalizedAds, value ? "1" : "0");

    private void RequestAds(bool isSplash)
    {
      Rewarded.RequestRewardedVideo();

      _interstitial.RequestInterstitial(isSplash);

#if !WITHOUT_BANNER
      Banner.RequestBanner();
#endif
    }

    private void InitializeBlocks()
    {
      InitializeReward();
#if !WITHOUT_BANNER
      InitializeBanner();
#endif
      InitializeInterstitial();
    }

    private void InitializeReward()
    {
      if (_isSng)
      {
#if MOCK_INSTEAD_YANDEX
        Rewarded = new RewardedDummy();
#else
        Rewarded = new YandexMobileAdsRewardedAd(_videoID, _analytics);
#endif
      }
      else 
        Rewarded = new Rewarded(_videoID, _analytics);
    }

    private void InitializeBanner()
    {
#if !WITHOUT_BANNER
      if (_isSng)
      {
#if MOCK_INSTEAD_YANDEX
        Banner = new BannerDummy();
#else
        Banner = new YandexMobileAdsBanner(_analytics, _bannerID);
#endif
      }
      else
        Banner = new Banner(_bannerID, _analytics);
#endif
    }

    private void InitializeInterstitial()
    {
      if (_isSng)
      {
#if MOCK_INSTEAD_YANDEX
        _interstitial = new InterstitialDummy();
        _onInitialize?.Invoke();
#else
        _interstitial = new YandexMobileAdsInterstitialAd(_analytics, _interstitialID, _onInitialize);
#endif
      }
      else
        _interstitial = new Interstitial(_interstitialSplashID, _interstitialID, _analytics, _onInitialize);
    }

    private void SetAdsBlocksState()
    {
#if !WITHOUT_BANNER
      Banner.SetEnabled(AdsEnabled);
#endif
      _interstitial.SetEnabled(AdsEnabled);
    }

    private void LoadAdsService()
    {
      if (!_isSng)
      {
#if ENABLED_ADMOB
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
#if UNITY_IOS
        MobileAds.SetiOSAppPauseOnBackground(true);
#endif
        MobileAds.Initialize(initStatus => { });
#endif        
      }
    }

    private void LoadSettings()
    {  
      _adsSettings = Resources.Load<SableGamesAdsSettings>(_isSng ? "Yandex Ads Settings" : "Sable Games Ads Settings");
      _placements = SableGamesAdsSettings.Convert(_adsSettings);
      _adsLogger.SetSettings(_adsSettings);
    }

    private void AdjustPlacements(PlacementType typePlacement)
    {
      if (_isSng)
      {
#if !MOCK_INSTEAD_YANDEX
        Adjust(typePlacement);
#endif
      }
      else
        Adjust(typePlacement);

      void Adjust(PlacementType placementType)
      {
#if !WITHOUT_BANNER
        _bannerID = _placements[placementType]["_bannerID"];
#endif
        _interstitialID = _placements[placementType]["_interstitialID"];
        _interstitialSplashID = _placements[placementType]["_interstitialSplashID"];
        _videoID = _placements[placementType]["_videoID"];
      }
    }
    
    private bool ShouldEnableAlternativeAds(string countryCode)
    {
      if (Application.isEditor)
        return false;
      
      return countryCode == "ru" || countryCode == "be" || countryCode == "uk";
    }
  }
}