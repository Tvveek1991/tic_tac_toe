using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Core_Logic.Screens;
using _Project.Scripts.Core_Logic.StatesMachine.Interfaces;
using _Project.Scripts.Core_Logic.StatesMachine.States;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.ScrollView;
using _Project.Scripts.Features.ScrollView.Serialized;
using _Project.Scripts.Features.ShopScreen.Content;
using _Project.Scripts.Features.ShopScreen.Data;
using _Project.Scripts.Features.ShopScreen.Mono;
using _Project.Scripts.Features.ShopScreen.Serialized;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Unlocker;
using AppacheAds.Scripts.Interfaces;
using AppacheAnalytics.Scripts.Interfaces;
using Sirenix.Utilities;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Features.ShopScreen
{
  public class ShopLogic : IShopLogic
  {
    private const string ShopTypeDataPath = "Features/Data/ShopData/Shop Types/";
    private const string GirlsDataPath = "Features/Data/GameGirlsData/Game Girls Data";

    private readonly IUiFactory _uiFactory;
    private readonly IShopFactory _shopFactory;

    private readonly IAnalytics _analytics;
    private readonly IAdsService _adsService;
    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;
    private readonly ICurrentScreen _currentScreen;
    private readonly IUnlockService _unlockService;
    private readonly IGameSaveSystem _gameSaveSystem;
    private readonly IGameMetaProvider _gameMetaProvider;
    private readonly IGameStateMachine _gameStateMachine;

    private ShopView _shopView;

    private readonly Dictionary<ShopItemType, UpgradeData[]> _itemsDataMap = new Dictionary<ShopItemType, UpgradeData[]>();
    private readonly Dictionary<ShopItemType, ScrollViewItemsView> _shopItemsView = new Dictionary<ShopItemType, ScrollViewItemsView>();
    private readonly Dictionary<ShopItemType, ScrollViewItemButton[]> _shopItemButtons = new Dictionary<ShopItemType, ScrollViewItemButton[]>();

    private readonly UpgradeData[] _upgradesData;
    private readonly ShopTypeData[] _shopTypesData;

    private readonly ContentViewLogic _contentViewLogic;

    private readonly IAssetProvider _assetProvider;

    private Dictionary<ShopItemType, ScrollViewSectionButton> _sectionButtons;

    public ShopLogic(IGameStateMachine gameStateMachine, IAssetProvider assetProvider, ICurrentScreen currentScreen,
      IGameSaveSystem gameSaveSystem, IAdsService adsService, IAnalytics analytics, ILocaleSystem localeSystem,
      IAudioService audioService, IUnlockService unlockService, IGameMetaProvider gameMetaProvider)
    {
      _analytics = analytics;
      _adsService = adsService;
      _localeSystem = localeSystem;
      _audioService = audioService;
      _currentScreen = currentScreen;
      _assetProvider = assetProvider;
      _unlockService = unlockService;
      _gameSaveSystem = gameSaveSystem;
      _gameSaveSystem = gameSaveSystem;
      _gameMetaProvider = gameMetaProvider;
      _gameStateMachine = gameStateMachine;

      _uiFactory = new UiFactory(_assetProvider);
      _shopFactory = new ShopFactory(_assetProvider);

      // var gameGirlsData = _assetProvider.GetResource<GameGirlsData>(GirlsDataPath);
      _shopTypesData = _assetProvider.GetAllResources<ShopTypeData>(ShopTypeDataPath);

      /*_upgradesData = gameGirlsData.GirlDatas
        .FirstOrDefault(item => item.Id == _gameMetaProvider.CurrentGirl)?.UpgradeDatas;*/

      foreach (ShopTypeData typeData in _shopTypesData)
      {
        UpgradeData[] itemsData = _upgradesData?.Where(item => item.ItemType == typeData.Type).ToArray();
        _itemsDataMap.Add(typeData.Type, itemsData);
      }

      _contentViewLogic = new ContentViewLogic();
      _contentViewLogic.Construct(_gameSaveSystem, _assetProvider);

      _currentScreen.SetCurrentScreen(ScreenNames.Shop);
    }

    public void CreateShopView(Action onCloseOverride = null)
    {
      _shopView = _shopFactory.CreateShopView();

      _shopView.Construct(onCloseOverride ?? CloseShop);

      CreateShopContent();

      CheckUnlocked();

      CreateSounds(_shopView);
    }

    public void ClearShopView()
    {     
      _currentScreen.SetCurrentScreen(ScreenNames.Home);
 
      _adsService.Banner.HideBanner();

      if (!_shopView) return;

      Object.Destroy(_shopView.gameObject);
    }

    private void OpenShop(ShopItemType type)
    {
      foreach (KeyValuePair<ShopItemType, ScrollViewItemsView> item in GetDeactivatedItemsViews(type))
        item.Value.Deactivate();

      _shopItemsView[type].Activate();
    }

    private void CreateShopContent()
    {
      _sectionButtons = new Dictionary<ShopItemType, ScrollViewSectionButton>();

      foreach (ShopTypeData t in _shopTypesData)
      {
        CreateSectionButton(t);
        CreateScrollItemsView(t);
        CreateItems(t);
      }

      OpenShop(ShopItemType.Look);
      _shopView.GetSectionButton(ShopItemType.Look).Button.onClick.Invoke();

      OpenFree();
    }

    private void CreateSectionButton(ShopTypeData t)
    {
      ScrollViewSectionButton shopButton = _shopView.GetSectionButton(t.Type);
      shopButton.Construct(_localeSystem, t.NameKey, delegate
      {
        UnselectSectionButtons();
        OpenShop(t.Type);
        SelectSectionButtons(t.Type);
      }, t.Icon_on, t.Icon_off);

      _sectionButtons.Add(t.Type, shopButton);
    }

    private void SelectSectionButtons(ShopItemType type) =>
      _sectionButtons[type].Select(true);

    private void UnselectSectionButtons() =>
      _sectionButtons.Values.ForEach(b => b.Select(false));

    private void CreateScrollItemsView(ShopTypeData t)
    {
      ScrollViewItemsView scrollViewItemsView = _shopFactory.CreateItemsView(_shopView.ItemViewsParent);
      _shopItemsView.Add(t.Type, scrollViewItemsView);
    }

    private void CreateItems(ShopTypeData t)
    {
      var shopItemButton = new List<ScrollViewItemButton>();
      for (var index = 0; index < _itemsDataMap[t.Type].Length; index++)
      {
        UpgradeData item = _itemsDataMap[t.Type][index];
        ScrollViewItemButton scrollViewItemButton = _shopFactory.CreateItemButton(_shopItemsView[t.Type].ContentParent);

        string level = item.Description.Equals("") ? String.Empty : " " + string.Format(_localeSystem.GetText("Level"), index + 1);
        string description = _localeSystem.GetText(item.Name) + level;

        scrollViewItemButton.Construct(_localeSystem, _unlockService,
          new ScrollViewItemData() { Id = item.Id, NameKey = item.Name, DescriptionKey = item.Description, Icon = item.Icon },
          () => OnClickItem(item), _audioService, description);

        shopItemButton.Add(scrollViewItemButton);
      }

      _shopItemButtons.Add(t.Type, shopItemButton.ToArray());
    }

    private void OpenFree()
    {
      foreach (ShopTypeData t in _shopTypesData)
      foreach (UpgradeData item in _itemsDataMap[t.Type])
        if (!IsUnlocked(item.Id) && _unlockService.GetUnlockType(item.Id) == UnlockType.Empty)
        {
          OnClickItem(item, true);
          
          /*_gameSaveSystem.Get().UpgradeContentChosenMoreThenOnce.Add(item.Id);
          _gameSaveSystem.Save();*/
        }
    }

    private void OnClickItem(UpgradeData itemData, bool fromInit = false)
    {
      if (IsUnlocked(itemData.Id))
        Choose(itemData, fromInit);
      else
        TryUnlock(itemData, fromInit);
    }

    private void Choose(UpgradeData itemData, bool fromInit = false)
    {
      if (!fromInit)
      {
        _analytics.Send("ev_main_selected", new Dictionary<string, object>
        {
          { "category_id", itemData.ItemType.ToString().ToLowerInvariant() },
          { "object_id", itemData.Id }
        });
      }

      foreach (ScrollViewItemButton item in _shopItemButtons[itemData.ItemType])
      {
        if (item.GetId() == itemData.Id)
        {
          item.Activate();
          if (!IsChosen(item.GetId()))
            _gameSaveSystem.Get().ChosenUpgradeContent.Add(itemData.Id);
        }
        else
        {
          item.Deactivate();
          if (IsChosen(item.GetId()))
            _gameSaveSystem.Get().ChosenUpgradeContent.Remove(item.GetId());
        }

        if(fromInit == false)
         _gameSaveSystem.Save();
      }

      if (fromInit == false)
      {
        
        
        /*if (itemData.ItemType == ShopItemType.Look && !_gameSaveSystem.Get().UpgradeContentChosenMoreThenOnce.Contains(itemData.Id))
        {
          _gameSaveSystem.Get().UpgradeContentChosenMoreThenOnce.Add(itemData.Id);
          _gameSaveSystem.Save();
        }
        else
          _contentViewLogic.Display();*/
      }
      else
        _contentViewLogic.Display();
    }

    private void TryUnlock(UpgradeData itemData, bool fromInit = false)
    {
      _unlockService.Unlock(itemData.Id, () =>
      {
        Unlock();
        GetItemButton(itemData)?.OpenContent(fromInit);
        OnClickItem(itemData, fromInit);

        AccessForUnlock();
        RefreshProgressBar();
        
        SendOpenEvent(itemData);
        
      }, type =>
        {
          /*if (type == UnlockType.Money)
            _moneyCounterLogic.ShowAddMoneyIfNotEnough();*/
        }
      );

      void Unlock()
      {
        _gameSaveSystem.Get().UnlockedUpgradeContent.Add(itemData.Id);
        _gameSaveSystem.Save();
      }
    }

    private void SendOpenEvent(UpgradeData itemData)
    {
      if (_unlockService.GetUnlockType(itemData.Id) == UnlockType.Empty)
        return;

      _analytics.Send("ev_main_open", new Dictionary<string, object>
      {
        { "category_id", itemData.ItemType.ToString().ToLowerInvariant() },
        { "object_id", itemData.Id }
      });
    }

    private void CheckUnlocked()
    {
      foreach (ScrollViewItemButton itemButton in GetAllItemButtons())
      {
        if (IsUnlocked(itemButton.GetId()))
        {
          itemButton.OpenContent(true);
          itemButton.SetLockButton(false);
        }

        if (IsChosen(itemButton.GetId()))
          itemButton.Activate();
        else
          itemButton.SetChooseState();
      }

      AccessForUnlock();
      RefreshProgressBar();
    }

    private void AccessForUnlock()
    {
      foreach (KeyValuePair<ShopItemType, ScrollViewItemButton[]> pair in _shopItemButtons)
      {
        var index = 0;

        List<ScrollViewItemButton> scrollViewItemButtons = pair.Value.ToList().FindAll(item => _unlockService.GetUnlockType(item.GetId()) == UnlockType.Money);

        foreach (ScrollViewItemButton itemButton in scrollViewItemButtons
          .Where(itemButton => IsUnlocked(itemButton.GetId()) && scrollViewItemButtons.IndexOf(itemButton) + 1 < scrollViewItemButtons.Count))
          index = scrollViewItemButtons.IndexOf(itemButton) + 1;
      }
    }

    private void RefreshProgressBar()
    {
      List<UpgradeData> upgradeDatas = _upgradesData.ToList().FindAll(item => _unlockService.GetUnlockType(item.Id) == UnlockType.Money);
      int boughtForMoney = upgradeDatas.Count(item => _gameSaveSystem.Get().UnlockedUpgradeContent.Contains(item.Id));
      _shopView.AddProgress(boughtForMoney, upgradeDatas.Count);
    }

    private void CloseShop()
    {
      _adsService.CurrentInterstitial.ShowInterstitial(ScreenNames.Home,
        _gameStateMachine.Enter<LoadHomeState>,
        _gameStateMachine.Enter<LoadHomeState>);
    }

    private void CreateSounds(ShopView shopView)
    {
      new ButtonSound(_audioService, shopView.BackButton.gameObject, AudioId.SoundButton);
    }

    private ScrollViewItemButton GetItemButton(UpgradeData itemData) =>
      _shopItemButtons[itemData.ItemType].FirstOrDefault(item => item.GetId() == itemData.Id);

    private IEnumerable<ScrollViewItemButton> GetAllItemButtons() =>
      _shopItemButtons.SelectMany(item => item.Value);

    private IEnumerable<KeyValuePair<ShopItemType, ScrollViewItemsView>> GetDeactivatedItemsViews(ShopItemType type) =>
      _shopItemsView.Where(item => item.Key != type);

    private bool IsUnlocked(string itemId) =>
      _gameSaveSystem.Get().UnlockedUpgradeContent.Contains(itemId);

    private bool IsChosen(string itemId) =>
      _gameSaveSystem.Get().ChosenUpgradeContent.Contains(itemId);
  }
}