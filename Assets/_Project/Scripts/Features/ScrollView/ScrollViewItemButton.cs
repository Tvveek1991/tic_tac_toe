using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Features.ScrollView.Serialized;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Localization.Mono;
using _Project.Scripts.Services.Unlocker;
using AppacheRemote.Scripts.Interfaces;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Project.Scripts.Features.ScrollView
{
  public class ScrollViewItemButton : MonoBehaviour, IPointerDownHandler
  {
    protected const float Duration = .3f;

    public Button Button;

    public GameObject Selector;

    public Image Icon;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Description;
    public BuyZone Buy;

    public RectTransform Blink;

    protected CanvasGroup ActiveBuyingGroup;

    protected ScrollViewItemData ScrollViewItemData;

    public CanvasGroup UnlockPanel;
    public CanvasGroup LockPanel;

    public bool SetNativeSizeIcon;

    public Animator Animator;

    public Image MainImage;
    public Image LockImage;
    public TextMeshProUGUI Info;
    public Image IconImage;
    public TextMeshProUGUI Counter;
    
    protected IAudioService AudioService;
    protected ILocaleSystem LocaleSystem;

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
      LocaleSystem = localeSystem;
      AudioService = audioService;

      SetBuyZoneDefaults(Buy.BoughtGroup);
      SetBuyZoneDefaults(Buy.AdvGroup);
      SetBuyZoneDefaults(Buy.MoneyGroup);
      SetBuyZoneDefaults(Buy.ChooseGroup);
      SetBuyZoneDefaults(Buy.IAPGroup);

      SwitchLockPanels(false);

      ScrollViewItemData = scrollViewItemData;

      SwitchSelector(false);

      Button.onClick.AddListener(() => onClickItem());

      SetIcon();
      SetDescription(description);

      SetLock();

      AnimateBlink();

      Localize(LocaleSystem);

      void SetIcon()
      {
        Icon.sprite = ScrollViewItemData.Icon;

        if (SetNativeSizeIcon)
          Icon.SetNativeSize();
      }

      void SetLock()
      {
        _type = unlockService.GetUnlockType(ScrollViewItemData.Id);

        if (_type == UnlockType.IAP)
        {
          Icon.sprite = ScrollViewItemData.IconLock; 
          Buy.IAPGarland.SetActive(true);
        }

        if (_type == UnlockType.Free)
          return;

        switch (_type)
        {
          case UnlockType.Money:
            ActiveBuyingGroup = Buy.MoneyGroup;
            break;
          case UnlockType.Advertising:
            ActiveBuyingGroup = Buy.AdvGroup;
            break;
          case UnlockType.IAP:
            ActiveBuyingGroup = Buy.IAPGroup;
            break;
        }

        ActiveBuyingGroup.DOFade(1f, Duration);

        Buy.Cost.text = unlockService.GetCost(ScrollViewItemData.Id).ToString();
      }
    }
    
    protected void SetBuyZoneDefaults(CanvasGroup target)
    {
      if (target != null)
        target.alpha = 0;
    }

    private void AnimateBlink()
    {
      if (!Blink)
        return;

      Sequence sequence = DOTween.Sequence();
      sequence.AppendInterval(7);
      sequence.Append(Blink.DOAnchorPos(new Vector2(60, 0), 1f));

      sequence.SetLoops(-1, LoopType.Restart);
    }

    protected virtual void SwitchSelector(bool value)
    {
      if (Selector != null)
        Selector.SetActive(value);
    }

    public virtual void OpenContent(bool fromInit = false)
    {
      ActiveBuyingGroup.DOFade(0f, Duration);

      PlayOpenContentSound(fromInit);

      if (_type == UnlockType.IAP)
      {
        Icon.sprite = ScrollViewItemData.Icon; 
        Buy.IAPGarland.SetActive(false);
      }
    }

    public void PlayOpenContentSound(bool fromInit = false)
    {
      if (fromInit == false)
        AudioService.Play(AudioId.SoundButton);
    }

    public void Activate()
    {
      Buy.BoughtGroup.DOFade(1, Duration);
      Buy.ChooseGroup.DOFade(0, Duration);
      ActiveBuyingGroup.DOFade(0, Duration);

      SwitchSelector(true);
    }

    public void Deactivate()
    {
      Buy.BoughtGroup.DOFade(0, Duration);
      Buy.ChooseGroup.DOFade(1, Duration);

      SwitchSelector(false);
    }

    public void SetChooseState()
    {
      if (Buy.ChooseGroup != null)
        Buy.ChooseGroup.DOFade(1, Duration);
    }

    public string GetId() =>
      ScrollViewItemData.Id;

    public void SetLockButton(bool isLock)
    {
      //Button.interactable = !isLock;
      if(LockPanel)
        LockPanel.gameObject.SetActive(isLock);
      SwitchLockPanels(isLock);
      
      LockImage?.gameObject.SetActive(isLock);
      Info?.gameObject.SetActive(isLock);
      IconImage?.gameObject.SetActive(!isLock);
      Counter?.gameObject.SetActive(!isLock);
    }

    public void SetName(string slotName) =>
      Name.text = slotName;

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

    public void OnPointerDown(PointerEventData eventData)
    {
      if (Animator)
        Animator.SetBool("Pressed", true);
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