using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Animation;
using _Project.Scripts.Features.Customization.Interface;
using _Project.Scripts.Features.Customization.Serialized;
using _Project.Scripts.Features.DisableAds;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio;
using _Project.Scripts.Services.Localization.Logic;
using AppacheAds.Scripts.Interfaces;
using AppacheAnalytics.Scripts.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.Features.Customization.Mono
{
  public class SkinsView : MonoBehaviour
  {
    private uint _amountCoins = 500; // TODO: вынести в конфиг баланса

    public Button BackButton;
    public Button AddCoinsButton;
    public Button GetRandomButton;
    public Button GetRandomAdButton;

    public Transform ActivePageTransform;
    public Transform UnActivePagesTransform;

    [SerializeField] private Transform background;

    [SerializeField] private Text randomCost;

    [Header("On Validated")] [ListDrawerSettings(IsReadOnly = true)] [SerializeField]
    private List<SkinsPage> _skinsPages;

    private DisableAdsLogic _disableAdsLogic;
    private IMoneyService _moneyService;
    private IAudioService _audioService;
    private IAdsService _ads;
    private ILocaleSystem _localeSystem;
    private ICurrentScreen _screenManager;

    private ApproveWindow _approveWindow;
    // private Popup _popup;

    private Action _onClickBack;

    [Inject]
    private void Inject(ILocaleSystem localeSystem) =>
      _localeSystem = localeSystem;

    public class Factory : PlaceholderFactory<UnityEngine.Object, Transform, SkinsView>
    {
    }

    private void OnValidate()
    {
      GenerateSkinPagesList();

      void GenerateSkinPagesList()
      {
        int skinTypesAmount = Enum.GetNames(typeof(SkinType)).Length;

        List<SkinsPage> _oldSkinsPages = new List<SkinsPage>(_skinsPages.ToArray());
        _skinsPages = new List<SkinsPage>();

        for (int i = 0; i < skinTypesAmount; i++)
        {
          SkinType skinType = (SkinType) i;
          SkinsPage pageOld = _oldSkinsPages.FirstOrDefault(s => s.SkinType == skinType);

          SkinsPage page = pageOld ?? new SkinsPage();
          page.SkinType = skinType;

          _skinsPages.Add(page);
        }
      }
    }

    public void Initialize(ISkins skins, ISkinsFactory skinsFactory, IMoneyService moneyService,
      IAdsService ads, IAudioService audioService, ICurrentScreen screenManager, Action<ISkinData> onSkinActivated, Action<ISkinData> onSkinReceived,
      DisableAdsLogic disableAdsLogic, Action onClickBack)
    {
      _approveWindow = FindObjectOfType<ApproveWindow>();

      if (_approveWindow == null)
        _approveWindow = skinsFactory.CreateApproveWindow(transform);

      _ads = ads;
      _moneyService = moneyService;
      _audioService = audioService;
      _screenManager = screenManager;
      name = name.Replace("(Clone)", "");
      _disableAdsLogic = disableAdsLogic;

      _onClickBack = onClickBack;

      _amountCoins = 100;
      SetRandomCost(_amountCoins);

      AddButtonListeners();
      InitializeSkinPages();

      new ButtonAnimation(AddCoinsButton.transform);
      new ButtonAnimation(GetRandomButton.transform);
      new ButtonAnimation(GetRandomAdButton.transform);

      // new AutoCloseBackZone(background, Hide);

      void InitializeSkinPages()
      {
        _skinsPages.ForEach(page => page.Initialize(skinsFactory, skins.GetSkinDatasByType(page.SkinType),
          ActivatePage, SkinPanelClick, _approveWindow, moneyService, onSkinActivated, onSkinReceived, GetRandomButton, _audioService, _localeSystem));

        ActivatePage(_skinsPages[0]);
      }

      void AddButtonListeners()
      {
        BackButton.onClick.AddListener(Hide);
        AddCoinsButton.onClick.AddListener(AddCoins);
        GetRandomButton.onClick.AddListener(GetRandom);
        GetRandomAdButton.onClick.AddListener(GetRandomAd);
      }

      // _popup = FindObjectOfType<Popup>();
    }

    private void SetRandomCost(uint amountCoins) =>
      randomCost.text = amountCoins.ToString();

    public void Show()
    {
      gameObject.SetActive(true);
      _screenManager.SetCurrentScreen("shop");
#if NO_ADS
      AddCoinsButton.transform.parent.gameObject.SetActive(false);
#endif
      CheckRandomButton();
    }

    private void ActivatePage(SkinsPage targetPage)
    {
      _skinsPages.ForEach(page => page.Deactivate(UnActivePagesTransform));
      targetPage.Activate(ActivePageTransform);

#if NO_ADS
      GetRandomButton.transform.parent.gameObject.SetActive(true);
#else
      AddCoinsButton.transform.parent.gameObject.SetActive(targetPage.SkinType != SkinType.Decor);
      GetRandomButton.transform.parent.gameObject.SetActive(targetPage.SkinType != SkinType.Decor);
      GetRandomAdButton.transform.parent.gameObject.SetActive(targetPage.SkinType == SkinType.Decor);
#endif
      CheckRandomButton();
    }

    private void SkinPanelClick(SkinsPage targetPage, SkinPanel targetPanel) =>
      targetPage.SkinPanelClick(targetPanel);

    private void Hide()
    {
      _onClickBack?.Invoke();
      _audioService.Play(AudioId.SoundButton);

      if (_ads.AdsEnabled)
        _ads.CurrentInterstitial.ShowInterstitial("start", () => gameObject.SetActive(false), () => gameObject.SetActive(false));
      else
        gameObject.SetActive(false);
      
      _screenManager.SetCurrentScreen("start");
    }

    private void AddCoins()
    {
      _audioService.Play(AudioId.SoundButton);
      _ads.Rewarded.ShowReward("other", () =>
        {
          // _disableAdsLogic.NotifyRewardedShow();

          _moneyService.Earn(_amountCoins);

          _amountCoins = (uint) (_amountCoins == 100 ? 200 : 100);
          SetRandomCost(_amountCoins);

          CheckRandomButton();
        },
        FindAndShowPopup);
    }

    private void GetRandom()
    {
      _audioService.Play(AudioId.SoundButton);
      GetActivePage(out SkinsPage page);
      StartCoroutine(page?.GetRandomsSkin(CheckRandomButton));
    }

    private void GetRandomAd()
    {
      _audioService.Play(AudioId.SoundButton);
      _ads.Rewarded.ShowReward("other", () =>
        {
          // _disableAdsLogic.NotifyRewardedShow();
          GetActivePage(out SkinsPage page);
          StartCoroutine(page?.GetRandomsSkin(CheckRandomButton, true));
        },
        FindAndShowPopup);
    }

    private void FindAndShowPopup()
    {
      // if (_popup == null)
      //   _popup = FindObjectOfType<Popup>();

      // if (_popup != null)
      //   _popup.OnRewardFailed();
    }

    private SkinsPage GetActivePage(out SkinsPage page)
    {
      page = _skinsPages.FirstOrDefault(p => p.IsActive());
      return page;
    }

    private void CheckRandomButton()
    {
      if (GetActivePage(out SkinsPage page) == null) return;

      if (page.AllSkinsIsReceived() || _moneyService.MoneyCount < page.GetPageUnlockRandomPrice() || page.IsActiveRandom)
        GetRandomButton.interactable = false;
      else
        GetRandomButton.interactable = true;

      if (page.SkinType != SkinType.Decor) return;

      GetRandomAdButton.interactable = !page.IsActiveRandom;
      if (page.AllSkinsIsReceived())
        GetRandomAdButton.gameObject.SetActive(false);
    }
  }
}