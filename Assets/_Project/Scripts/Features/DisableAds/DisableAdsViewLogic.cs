using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Animation;
using _Project.Scripts.Features.DisableAds.Mono;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio.Enum;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Localization.Mono;
using Sirenix.Utilities;

namespace _Project.Scripts.Features.DisableAds
{
  public class DisableAdsViewLogic : IDisableAdsViewLogic
  {
    private const string ShowAdsTextKey = "DisableAds.Show";

    private readonly DisableAdsWindow _disableAdsWindow;

    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;
    private readonly IGameSaveSystem _gameSaveSystem;

    public DisableAdsViewLogic(DisableAdsWindow disableAdsWindow, ILocaleSystem localeSystem, IAudioService audioService, IGameSaveSystem gameSaveSystem)
    {
      _disableAdsWindow = disableAdsWindow;
      _gameSaveSystem = gameSaveSystem;
      _localeSystem = localeSystem;
      _audioService = audioService;
    }

    public void Construct(string price, Action onShowAds, Action onClose, Action onContinue, Action onPurchase)
    {
      _disableAdsWindow.ShowBtn.onClick.AddListener((() => _audioService.Play(AudioId.Click)));
      _disableAdsWindow.CloseBtn.onClick.AddListener((() => _audioService.Play(AudioId.ClickBack)));
      _disableAdsWindow.ContinueBtn.onClick.AddListener((() => _audioService.Play(AudioId.Click)));
      _disableAdsWindow.PurchaseBtn.onClick.AddListener((() => _audioService.Play(AudioId.Click)));

      _disableAdsWindow.ShowBtn.onClick.AddListener(delegate { onShowAds?.Invoke(); });
      _disableAdsWindow.CloseBtn.onClick.AddListener(delegate { onClose?.Invoke(); });
      _disableAdsWindow.ContinueBtn.onClick.AddListener(delegate { onContinue?.Invoke(); });
      _disableAdsWindow.PurchaseBtn.onClick.AddListener(delegate { onPurchase?.Invoke(); });

      _disableAdsWindow.PurchasePrice.text = string.Format(_localeSystem.GetText("DisableAds.BuyFor"), price);

      Localize(_localeSystem);

      new ButtonAnimation(_disableAdsWindow.ShowBtn.transform);
      new ButtonAnimation(_disableAdsWindow.CloseBtn.transform);
      new ButtonAnimation(_disableAdsWindow.ContinueBtn.transform);
      new ButtonAnimation(_disableAdsWindow.PurchaseBtn.transform);
    }

    public void UpdateAdsWatchedProgress(int progress, int max) =>
      _disableAdsWindow.AdsWatchedProgress.text = $"{_localeSystem.GetText(ShowAdsTextKey)} {progress + "/" + max}";

    public void UpdateDisableAdsTimer(int sec)
    {
      TimeSpan t = TimeSpan.FromSeconds(sec);
      string hours = t.Hours < 10 ? $"0{t.Hours}" : t.Hours.ToString();
      string minutes = t.Minutes < 10 ? $"0{t.Minutes}" : t.Minutes.ToString();
      string seconds = t.Seconds < 10 ? $"0{t.Seconds}" : t.Seconds.ToString();

      if (_disableAdsWindow)
        _disableAdsWindow.DisableAdsTimers.ForEach(t => t.text = $"{hours}:{minutes}:{seconds}");
    }

    public void OpenWindow()
    {
      _disableAdsWindow.gameObject.SetActive(true);
      AppearAnimation();

      _disableAdsWindow.SwapperBackIcon.SetIcon(_gameSaveSystem.Get().IsDisableAdsFirstTime);
    }

    private void AppearAnimation()
    {
    }

    public void CloseWindow()
    {
      DisappearAnimation();
    }

    private void DisappearAnimation()
    {
      _disableAdsWindow.gameObject.SetActive(false);
    }

    public void ShowAdsOfferEnabled()
    {
      _disableAdsWindow.AdsOfferEnabled.SetActive(true);
      _disableAdsWindow.AdsOfferDisabled.SetActive(false);
    }

    public void ShowAdsOfferDisabled()
    {
      _disableAdsWindow.AdsOfferEnabled.SetActive(false);
      _disableAdsWindow.AdsOfferDisabled.SetActive(true);
    }

    public void ShowInfoPanelBefore()
    {
      _disableAdsWindow.InfoPanelBefore.SetActive(true);
      _disableAdsWindow.InfoPanelAfter.SetActive(false);

      _disableAdsWindow.ShowBtn.gameObject.SetActive(true);
      _disableAdsWindow.ContinueBtn.gameObject.SetActive(false);
    }

    public void ShowInfoPanelAfter()
    {
      _disableAdsWindow.InfoPanelBefore.SetActive(false);
      _disableAdsWindow.InfoPanelAfter.SetActive(true);

      _disableAdsWindow.ShowBtn.gameObject.SetActive(false);
      _disableAdsWindow.ContinueBtn.gameObject.SetActive(true);
    }

    private void Localize(ILocaleSystem localeSystem)
    {
      List<LocalizedMainText> localizedTexts = _disableAdsWindow.GetComponentsInChildren<LocalizedMainText>(true).ToList();
      localizedTexts.ForEach(item =>
      {
        item.Construct(localeSystem);
        item.Localize();
      });
    }
  }
}