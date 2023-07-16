using System;
using _Project.Scripts.Features.NotLoaded;
using _Project.Scripts.Services._data;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.InApps.Scripts;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Timer;
using _Project.Scripts.Services.Timer.Interfaces;
using _Project.Scripts.Timer.Interfaces;
using SableGamesAds.Scripts.Interfaces;
using SableGamesAnalytics.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Purchasing;

namespace _Project.Scripts.Features.DisableAds
{
  public class DisableAdsLogic : ICallbackExecutor, IInAppCallback
  {
    private const int TargetAds = 2;
    private const string TimerGuid = "DisableAdsLogic24Timer";
    private const int DelaySec = 86400;

    public ProductId ProductsId { get; set; }

    private readonly IDisableAdsFactory _disableAdsFactory;
    
    private readonly IGameSaveSystem _gameSaveSystem;
    private readonly IAnalytics _analytics;
    private readonly IAdsService _adsService;
    private readonly IInAppService _inAppService;
    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;
    private readonly IAssetProvider _assetProvider;

    private readonly Action<bool> _onDisableAds;

    private readonly IDisableAdsViewLogic _disableAdsViewLogic;

    private readonly ITimerController _timerController;
    private readonly Action<bool> _switchDisableAdsButton;
    private readonly Action _onClose;
    
    private bool WasDisabled
    {
      get => PlayerPrefs.GetInt("DisableAdsLogic.WasDisabled", 0) == 1;
      set => PlayerPrefs.SetInt("DisableAdsLogic.WasDisabled", value ? 1 : 0);
    }

    public DisableAdsLogic(IAssetProvider assetProvider, IGameSaveSystem gameSaveSystem, IAnalytics analytics,
      IAdsService adsService, ILocaleSystem localeSystem, IInAppService inAppService, IAudioService audioService, 
      Action<bool> onDisableAds, Action<bool> switchDisableAdsButton, Action onClose = null)
    {
      _assetProvider = assetProvider;
      _switchDisableAdsButton = switchDisableAdsButton;
      ProductsId = ProductId.noads;
      
      _inAppService = inAppService;
      _localeSystem = localeSystem;
      _gameSaveSystem = gameSaveSystem;
      _analytics = analytics;
      _adsService = adsService;
      _onDisableAds = onDisableAds;
      _onClose = onClose;
      
      _disableAdsFactory = new DisableAdsFactory(assetProvider, _localeSystem, audioService);
      
      _disableAdsViewLogic = _disableAdsFactory.SpawnDisableAdsWindow();
      
      string price = _inAppService?.GetPrice(ProductsId.ToString());
      
      _disableAdsViewLogic.Construct(price, OnShowAds, OnClose, OnContinue, OnPurchase);
      
      _timerController = new TimerController();
      _timerController.LaunchService();

      Debug.Log($"IsUnderTimer {_timerController.IsUnderTimer(TimerGuid)}");

      if (IsProductPurchased())
        DisableAds(true, true);
      else if (_timerController.IsUnderTimer(TimerGuid))
      {
        _timerController.AddCallback(this);
        DisableAds(true);
      }
      else
      {
        if (WasDisabled)
          EnableAds();
      }
      
      FirstOpen();
    }

    private void FirstOpen()
    {
      if (!_gameSaveSystem.Get().IsDisableAdsFirstTime)
        return;

      _gameSaveSystem.Get().IsDisableAdsFirstTime = false;
      _gameSaveSystem.Save();

      Open();
    }

    public void Cleanup() => 
      _disableAdsFactory.Clear();

    public void Open()
    {
      if (IsProductPurchased())
      {
        Debug.Log("DisableAds.Open IsProductPurchased");
        return;
      }
      
      SetDisableAdsProgress();

      _disableAdsViewLogic.ShowInfoPanelBefore();
      
      if (_timerController.IsUnderTimer(TimerGuid))
        _disableAdsViewLogic.ShowAdsOfferDisabled();
      else
        _disableAdsViewLogic.ShowAdsOfferEnabled();

      // _analytics.Send("ad_offer_show");
      _disableAdsViewLogic.OpenWindow();
      
      // _adsService.Banner?.HideBanner();
    }

    private void Close()
    {
      Debug.Log("Test " + _disableAdsViewLogic);
      _disableAdsViewLogic.CloseWindow();
      
      // _adsService.Banner?.ShowBanner();
    }

    private void SetDisableAdsProgress() =>
      _disableAdsViewLogic.UpdateAdsWatchedProgress(_gameSaveSystem.Get().DisableAdsProgress, TargetAds);

    private void OnPurchase()
    {
      // _analytics.Send("btn_no_ads_24h");
      // _inAppService.Purchase(ProductsId.ToString()); while test

      OnPurchaseComplete(null);
    }

    private void OnContinue() =>
      Close();

    private void OnClose()
    {
      // _analytics.Send("ad_offer_skipped");
      Close();
      // _adsService.CurrentInterstitial.ShowInterstitial(ScreenNames.Home, () => _onClose?.Invoke(), () => _onClose?.Invoke());
    }

    private void OnShowAds()
    {
      _analytics.Send("btn_no_ads_24h");
      _adsService.Rewarded.ShowReward("DisableAds", OnReward, OnNotLoadedAds);
    }

    private void OnReward()
    {
      GameSaveData save = _gameSaveSystem.Get();
      save.DisableAdsProgress = Mathf.Clamp(save.DisableAdsProgress + 1, 0, TargetAds);
      _gameSaveSystem.Save();

      SetDisableAdsProgress();

      if (save.DisableAdsProgress == TargetAds)
      {
        DisableAds();
        CreateTimer(TimerGuid, DelaySec);
      }
      
      _disableAdsViewLogic.OpenWindow();
    }

    private void OnNotLoadedAds()
    {
      var notLoadedViewLogic = new NotLoadedWindowLogic(_localeSystem, _assetProvider);
      notLoadedViewLogic.Open();
    }

    private void EnableAds()
    {
      if (IsProductPurchased())
      {
        Debug.Log("DisableAds.EnableAds IsProductPurchased");
        return;
      }
      
      GameSaveData save = _gameSaveSystem.Get();
      save.IsDisableAdsActive = false;
      save.DisableAdsProgress = save.DisableAdsProgress >= TargetAds ? 0 : save.DisableAdsProgress;
      _gameSaveSystem.Save();

      _onDisableAds?.Invoke(false);

      SetDisableAdsProgress();
      _disableAdsViewLogic.ShowInfoPanelBefore();
      _disableAdsViewLogic.ShowAdsOfferEnabled();

      if (!WasDisabled)
        return;

      WasDisabled = false;

      /*_adsService.EnableAds(true);
#if !WITHOUT_BANNER
      _adsService.Banner.HideBanner();
#endif*/
      
      _switchDisableAdsButton.Invoke(true);
    }

    private void DisableAds(bool fromConstruct = false, bool purchase = false)
    {
      WasDisabled = true;

      GameSaveData save = _gameSaveSystem.Get();
      save.IsDisableAdsActive = true;
      _gameSaveSystem.Save();

      _onDisableAds?.Invoke(true);
      // _adsService.DisableAds();
      
      if (purchase)
      {
        _switchDisableAdsButton.Invoke(false);
        Close();
        return;
      }

      if(fromConstruct)
        _disableAdsViewLogic.ShowInfoPanelBefore();
      else
        _disableAdsViewLogic.ShowInfoPanelAfter();
      
      _disableAdsViewLogic.ShowAdsOfferDisabled();
    }

    private void CreateTimer(string guid, int seconds)
    {
      DateTime targetDatetime = _timerController.TimeProvider.GetCurrentTime().AddSeconds(seconds);
      _timerController.AddCallback(this);
      _timerController.CreateTimer(guid, targetDatetime);
    }

    public string GetTimerGuid() =>
      TimerGuid;

    public void ExecuteWhenTick(int seconds) =>
      _disableAdsViewLogic.UpdateDisableAdsTimer(seconds);

    public void ExecuteWhenComplete() =>
      EnableAds();

    public void SetLock(bool isLocked)
    {
    }

    public void Destroy()
    {
    }
    
    private bool IsProductPurchased()
    {
      if (!Application.isEditor) return _inAppService.IsProductPurchased(ProductsId.ToString());
      GameSaveData save = _gameSaveSystem.Get();
      return save.DisableAdsIsPurchased;
    }

    public void OnPurchaseComplete(Product purchased)
    {
      Debug.Log("DisableAds OnPurchaseComplete");
      
      DisableAds(false, true);
      GameSaveData save = _gameSaveSystem.Get();
      save.DisableAdsIsPurchased = true;
      _gameSaveSystem.Save();
    }

    public void OnPurchaseFailed()
    {
      Debug.Log("DisableAds OnPurchaseFailed");
    }

    public void OnNotPurchased()
    {
      Debug.Log("DisableAds OnNotPurchased");
    }
  }
}