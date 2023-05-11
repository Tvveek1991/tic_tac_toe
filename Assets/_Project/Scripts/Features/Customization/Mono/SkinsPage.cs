using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Features.Customization.Interface;
using _Project.Scripts.Features.Customization.Serialized;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio;
using _Project.Scripts.Services.Localization.Logic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.Customization.Mono
{
  [Serializable]
  public class SkinsPage
  {
    public bool IsActiveRandom { get; private set; }
    
    [ReadOnly] public SkinType SkinType;
    [SerializeField] public SpriteState SpriteState;
    public GameObject ButtonObject;
    public GameObject PageObject;
    [ReadOnly] public GameObject ScrollViewObject;
    [ReadOnly] public SkinPanel[] SkinPanles;

    private int _pageUnlockRandomPrice;
    private IMoneyService _moneyService;
    private ApproveWindow _approveWindow;
    private Text _costRandomUnlock;
    private Text _pageName;

    private Action<ISkinData> _onSkinActivated;
    private Action<ISkinData> _onSkinReceived;
    private ILocaleSystem _localeSystem;

    private Color _unActivePageButtonColor = new Color(0.65f, 0.74f, 0.87f);
    
    private float _randomTime = .25f;

    public void Initialize(ISkinsFactory skinsFactory, ISkinData[] skinDatas,
      Action<SkinsPage> onClickPageButton, Action<SkinsPage, SkinPanel> onClickSkinPanel, ApproveWindow approveWindow, IMoneyService moneyService,
      Action<ISkinData> onSkinActivated, Action<ISkinData> onSkinReceived, Button randomButton, IAudioService audioService, ILocaleSystem localeSystem)
    {
      _localeSystem = localeSystem;
      
      ButtonObject.GetComponent<Button>().onClick.AddListener(delegate
      {
        onClickPageButton(this);
        audioService.Play(AudioId.SoundButton);
      });

      var localizedText = GetLocalizedTextForSkinType(SkinType, _localeSystem);

      _pageName = ButtonObject.GetComponentInChildren<Text>();
      _pageName.text = localizedText; // TODO: убрать

      _approveWindow = approveWindow;
      _moneyService = moneyService;

      _costRandomUnlock = randomButton.GetComponentInChildren<Text>();

      _onSkinActivated = onSkinActivated;
      _onSkinReceived = onSkinReceived;

      Transform content;
      InstantiateScrollView();
      InstantiateSkinPanels();
      ChooseDefaultSkin();

      _pageUnlockRandomPrice = GetPrice(SkinPanles.First(item => GetPrice(item.GetSkinData()) != 0).GetSkinData());
      _costRandomUnlock.text = _pageUnlockRandomPrice.ToString();

      void InstantiateScrollView()
      {
        ScrollViewObject = skinsFactory.CreateScrollView(PageObject.transform);
        content = ScrollViewObject.GetComponent<ScrollRect>().content;

        ScrollViewObject.gameObject.name = ScrollViewObject.gameObject.name.Replace("(Clone)", "");
      }

      void InstantiateSkinPanels()
      {
        var skinPanelsList = new List<SkinPanel>();
        foreach (ISkinData skinData in skinDatas)
        {
          SkinPanel skinPanel = skinsFactory.CreateSkinPanel(content);
          skinPanel.Initialize(skinData, this, onClickSkinPanel);

          skinPanelsList.Add(skinPanel);
        }

        CreateEmptyPanels(skinsFactory, skinPanelsList, content);

        SkinPanles = skinPanelsList.ToArray();
      }

      void ChooseDefaultSkin()
      {
        SkinPanles.Where(skinPanel => GetPrice(skinPanel.GetSkinData()) == 0 || skinPanel.GetSkinData().IsReceived()).ToList().ForEach(ReceiveSkin);

        SkinPanel activeOpenSkin = SkinPanles.FirstOrDefault(skinPanel => skinPanel.GetSkinData().IsActivated());
        SkinPanel firstOpenSkin = SkinPanles.FirstOrDefault(skinPanel => skinPanel.GetSkinData().IsReceived());

        if (activeOpenSkin != null)
          ActivateSkin(activeOpenSkin);
        else if (firstOpenSkin != null)
          ActivateSkin(firstOpenSkin);
        else
        {
          if (SkinPanles.Length <= 0) return;
          
          ReceiveSkin(SkinPanles[0]);
          ActivateSkin(SkinPanles[0]);
        }
      }
    }

    public int GetPageUnlockRandomPrice() =>
      _pageUnlockRandomPrice;

    public void Activate(Transform parent)
    {
      ButtonObject.transform.SetParent(parent);
      _costRandomUnlock.text = _pageUnlockRandomPrice.ToString();
      // SpriteState.Enable();
      PageObject.SetActive(true);
      
      _pageName.color = Color.white;
    }

    public void Deactivate(Transform parent)
    {
      ButtonObject.transform.SetParent(parent);
      // SpriteState.Disable();
      PageObject.SetActive(false);
      
      _pageName.color = _unActivePageButtonColor;
    }

    public bool IsActive() =>
      PageObject.activeInHierarchy;

    public void SkinPanelClick(SkinPanel skinPanel)
    {
      ISkinData skinData = skinPanel.GetSkinData();

      if (skinData.IsReceived())
        ActivateSkin(skinPanel);
    }

    public IEnumerator GetRandomsSkin(Action callback, bool isFree = false)
    {
      if (_moneyService.MoneyCount >= _pageUnlockRandomPrice || isFree)
      {
        SkinPanel[] skinPanelsForReceive = SkinPanles.Where(skin => skin.GetSkinData().IsReceived() == false).ToArray();
        SkinPanel skinForReceive = null;

        IsActiveRandom = true;
        callback?.Invoke();

        var time = 0f;
        while (time != _randomTime)
        {
          yield return new WaitForSeconds(time);

          int random = UnityEngine.Random.Range(0, skinPanelsForReceive.Length);
          skinForReceive = skinPanelsForReceive[random];

          if (skinPanelsForReceive.Length == 1)
            break;

          ActivateSkin(skinForReceive, true);

          time = Mathf.MoveTowards(time, _randomTime, Time.deltaTime);
        }

        IsActiveRandom = false;

        /*if(!isFree)
          _moneyService.Spend((uint) _pageUnlockRandomPrice);*/
        
        ReceiveSkin(skinForReceive);
        ActivateSkin(skinForReceive);

        callback?.Invoke();
      }
    }

    public bool AllSkinsIsReceived() =>
      SkinPanles.Count(skin => skin.GetSkinData().IsReceived() == false) == 0;

    private void ActivateSkin(SkinPanel targetSkinPanel, bool isIterateOver = false)
    {
      SkinPanles.ToList().ForEach(skinPanel => skinPanel.Deactivate());
      targetSkinPanel.Activate();

      if(!isIterateOver)
        _onSkinActivated.Invoke(targetSkinPanel.GetSkinData());
    }

    private void ReceiveSkin(SkinPanel targetSkinPanel)
    {
      targetSkinPanel.ChangeStatusToReceive();

      _onSkinReceived.Invoke(targetSkinPanel.GetSkinData());
    }
    
    private void CreateEmptyPanels(ISkinsFactory skinsFactory, List<SkinPanel> skinPanelsList, Transform content)
    {
      if (skinPanelsList.Count >= 9) return;

      int emptyPanelCount = 9 - skinPanelsList.Count;
      while (emptyPanelCount-- != 0)
      {
        SkinPanel skinPanel = skinsFactory.CreateSkinPanel(content);
        skinPanel.Initialize(_localeSystem);
      }
    }

    private string GetLocalizedTextForSkinType(SkinType skinType, ILocaleSystem localeSystem)
    {
      string key;
      if (skinType == SkinType.Blender)
        key = "Blenders";
      else if (skinType == SkinType.Glass)
        key = "Glasses";
      else if (skinType == SkinType.Decor)
        key = "Decor";
      else
        throw new ArgumentOutOfRangeException();

      var text = localeSystem.GetText(key);
      return text;
    }

    private int GetPrice(ISkinData skinDataConfig) =>
      skinDataConfig.GetTestPrice();
  }
}