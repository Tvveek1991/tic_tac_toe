using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Features.ScrollView.Serialized;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio.Enum;
using _Project.Scripts.Services.Audio.Executor;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Localization.Mono;
using _Project.Scripts.Services.Unlocker;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.ScrollView
{
  public class ScrollViewItemButton : MonoBehaviour
  {
    private const float Duration = .3f;

    public Button Button;

    public Image Selector;

    public Image Icon;
    public TextMeshProUGUI Description;
    public BuyZone Buy;

    public CanvasGroup UnlockPanel;
    public CanvasGroup LockPanel;

    public bool SetNativeSizeIcon;

    private CanvasGroup _activeBuyingGroup;

    private ScrollViewItemData _scrollViewItemData;

    private IAudioService _audioService;
    private ILocaleSystem _localeSystem;

    private UnlockType _type;

    [Serializable]
    public struct BuyZone
    {
      public TextMeshProUGUI Cost;
      public CanvasGroup BoughtGroup;
      public CanvasGroup AdvGroup;
      public CanvasGroup MoneyGroup;
      public CanvasGroup ChooseGroup;
      public CanvasGroup IAPGroup;
      public GameObject IAPGarland;
    }

    public virtual void Construct(ILocaleSystem localeSystem, IUnlockService unlockService, ScrollViewItemData scrollViewItemData,
      Action onClickItem, IAudioService audioService, string description)
    {
      _localeSystem = localeSystem;
      _audioService = audioService;

      // SetBuyZoneDefaults(Buy.BoughtGroup);
      SetBuyZoneDefaults(Buy.AdvGroup);
      SetBuyZoneDefaults(Buy.MoneyGroup);
      // SetBuyZoneDefaults(Buy.ChooseGroup);
      SetBuyZoneDefaults(Buy.IAPGroup);

      SwitchLockPanels(false);

      _scrollViewItemData = scrollViewItemData;

      SwitchSelector(false);

      Button.onClick.AddListener(() => onClickItem());

      // SetIcon();
      SetDescription(description);

      SetLock();

      Localize(_localeSystem);
      new ButtonSound(audioService, Button.gameObject, AudioId.Click);

      void SetIcon()
      {
        Icon.sprite = _scrollViewItemData.Icon;

        if (SetNativeSizeIcon)
          Icon.SetNativeSize();
      }

      void SetLock()
      {
        _type = unlockService.GetUnlockType(_scrollViewItemData.Id);

        if (_type == UnlockType.IAP)
        {
          Icon.sprite = _scrollViewItemData.IconLock;
          Buy.IAPGarland.SetActive(true);
        }

        if (_type == UnlockType.Free)
          return;

        _activeBuyingGroup = _type switch
        {
          UnlockType.Money => Buy.MoneyGroup,
          UnlockType.Advertising => Buy.AdvGroup,
          UnlockType.IAP => Buy.IAPGroup,
          _ => _activeBuyingGroup
        };

        _activeBuyingGroup.DOFade(1f, Duration);

        Buy.Cost.text = unlockService.GetCost(_scrollViewItemData.Id).ToString();
      }
    }

    private void SetBuyZoneDefaults(CanvasGroup target)
    {
      if (target != null)
        target.alpha = 0;
    }

    protected virtual void SwitchSelector(bool value) =>
      Selector.DOFade(value ? 1f : .05f, Duration);

    public virtual void OpenContent(bool fromInit = false)
    {
      _activeBuyingGroup.DOFade(0f, Duration);

      PlayOpenContentSound(fromInit);

      if (_type == UnlockType.IAP)
      {
        Icon.sprite = _scrollViewItemData.Icon;
        Buy.IAPGarland.SetActive(false);
      }
    }

    private void PlayOpenContentSound(bool fromInit = false)
    {
      if (fromInit == false)
        _audioService.Play(AudioId.Click);
    }

    public void Activate()
    {
      // Buy.BoughtGroup.DOFade(1, Duration);
      // Buy.ChooseGroup.DOFade(0, Duration);
      _activeBuyingGroup.DOFade(0, Duration);

      SwitchSelector(true);
    }

    public void Deactivate()
    {
      // Buy.BoughtGroup.DOFade(0, Duration);
      // Buy.ChooseGroup.DOFade(1, Duration);

      SwitchSelector(false);
    }

    public void SetChooseState()
    {
      // Buy.ChooseGroup.DOFade(1, Duration);
    }

    public string GetId() =>
      _scrollViewItemData.Id;

    public void SetLockButton(bool isLock)
    {
      if (LockPanel)
        LockPanel.gameObject.SetActive(isLock);
      SwitchLockPanels(isLock);
    }

    protected virtual void SetDescription(string description) =>
      Description.text = description;

    private void SwitchLockPanels(bool isLock)
    {
      if (UnlockPanel == null || LockPanel == null)
        return;

      if (isLock)
      {
        UnlockPanel.alpha = 0;
        LockPanel.alpha = 1;
      }
      else
      {
        UnlockPanel.alpha = 1;
        LockPanel.alpha = 0;
      }
    }

    private void Localize(ILocaleSystem localeSystem)
    {
      List<LocalizedMainText> localizedTexts = transform.GetComponentsInChildren<LocalizedMainText>(true).ToList();
      localizedTexts.ForEach(item =>
      {
        item.Construct(localeSystem);
        item.Localize();
      });
    }
  }
}