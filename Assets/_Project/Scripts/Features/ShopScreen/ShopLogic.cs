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
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Features.ShopScreen
{
  public class ShopLogic : IShopLogic
  {
    private const string ShopTypeDataPath = "Data/Shop/Shop Types/";
    private const string ShopItemDataPath = "Data/Shop/Shop Items/";

    private readonly IUiFactory _uiFactory;
    private readonly IShopFactory _shopFactory;

    private readonly IAnalytics _analytics;
    private readonly IAdsService _adsService;
    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;
    private readonly IAssetProvider _assetProvider;
    private readonly ICurrentScreen _currentScreen;
    private readonly IUnlockService _unlockService;
    private readonly IGameSaveSystem _gameSaveSystem;
    private readonly IGameMetaProvider _gameMetaProvider;
    private readonly IGameStateMachine _gameStateMachine;

    private ShopView _shopView;

    private readonly Dictionary<ItemType, ShopItemData[]> _itemsDataMap = new();
    private readonly Dictionary<ItemType, ScrollViewItemsView> _shopItemsView = new();
    private readonly Dictionary<ItemType, ScrollViewItemButton[]> _shopItemButtons = new();

    private readonly ShopItemData[] _shopItemData;
    private readonly ShopTypeData[] _shopTypesData;


    private Dictionary<ItemType, ScrollViewSectionButton> _sectionButtons;

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

      _shopItemData = _assetProvider.GetAllResources<ShopItemData>(ShopItemDataPath);
      _shopTypesData = _assetProvider.GetAllResources<ShopTypeData>(ShopTypeDataPath);

      foreach (ShopTypeData typeData in _shopTypesData)
      {
        ShopItemData[] itemsData = _shopItemData?.Where(item => item.ItemType == typeData.Type).ToArray();
        _itemsDataMap.Add(typeData.Type, itemsData);
      }

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

    private void OpenShop(ItemType type)
    {
      foreach (KeyValuePair<ItemType, ScrollViewItemsView> item in GetDeactivatedItemsViews(type))
        item.Value.Deactivate();

      _shopItemsView[type].Activate();
    }

    private void CreateShopContent()
    {
      _sectionButtons = new Dictionary<ItemType, ScrollViewSectionButton>();

      foreach (ShopTypeData t in _shopTypesData)
      {
        CreateSectionButton(t);
        CreateScrollItemsView(t);
        CreateItems(t);
      }

      OpenShop(ItemType.SymbolX);
      _shopView.GetSectionButton(ItemType.SymbolX).Button.onClick.Invoke();

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

    private void SelectSectionButtons(ItemType type) =>
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
        ShopItemData item = _itemsDataMap[t.Type][index];
        ScrollViewItemButton scrollViewItemButton = _shopFactory.CreateItemButton(_shopItemsView[t.Type].ContentParent);

        string level = item.Description.Equals("") ? string.Empty : "112 " /*+ string.Format(_localeSystem.GetText("Level"), index + 1)*/;
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
      foreach (ShopItemData item in _itemsDataMap[t.Type])
        if (!IsUnlocked(item.Id) && _unlockService.GetUnlockType(item.Id) == UnlockType.Free)
          OnClickItem(item, true);
    }

    private void OnClickItem(ShopItemData itemData, bool fromInit = false)
    {
      if (IsUnlocked(itemData.Id))
        Choose(itemData, fromInit);
      else
        TryUnlock(itemData, fromInit);
    }

    private void Choose(ShopItemData itemData, bool fromInit = false)
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
            _gameSaveSystem.Get().ChosenShopContent.Add(itemData.Id);
        }
        else
        {
          item.Deactivate();
          if (IsChosen(item.GetId()))
            _gameSaveSystem.Get().ChosenShopContent.Remove(item.GetId());
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
      // else
        // _contentViewLogic.Display();
    }

    private void TryUnlock(ShopItemData itemData, bool fromInit = false)
    {
      _unlockService.Unlock(itemData.Id, () =>
      {
        Unlock();
        GetItemButton(itemData)?.OpenContent(fromInit);
        OnClickItem(itemData, fromInit);

        AccessForUnlock();
        RefreshProgressBar();
        
        SendOpenEvent(itemData);
        
      });

      void Unlock()
      {
        _gameSaveSystem.Get().UnlockedShopContent.Add(itemData.Id);
        _gameSaveSystem.Save();
      }
    }

    private void SendOpenEvent(ShopItemData itemData)
    {
      if (_unlockService.GetUnlockType(itemData.Id) == UnlockType.Free)
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
      foreach (KeyValuePair<ItemType, ScrollViewItemButton[]> pair in _shopItemButtons)
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
      List<ShopItemData> upgradeDatas = _shopItemData.ToList().FindAll(item => _unlockService.GetUnlockType(item.Id) == UnlockType.Money);
      int boughtForMoney = upgradeDatas.Count(item => _gameSaveSystem.Get().UnlockedShopContent.Contains(item.Id));
      _shopView.AddProgress(boughtForMoney, upgradeDatas.Count);
    }

    private void CloseShop()
    {
      _adsService.CurrentInterstitial.ShowInterstitial(ScreenNames.Home,
        _gameStateMachine.Enter<LoadHomeState>,
        _gameStateMachine.Enter<LoadHomeState>);
    }

    private void CreateSounds(ShopView shopView) => 
      new ButtonSound(_audioService, shopView.BackButton.gameObject, AudioId.SoundButton);

    private ScrollViewItemButton GetItemButton(ShopItemData itemData) =>
      _shopItemButtons[itemData.ItemType].FirstOrDefault(item => item.GetId() == itemData.Id);

    private IEnumerable<ScrollViewItemButton> GetAllItemButtons() =>
      _shopItemButtons.SelectMany(item => item.Value);

    private IEnumerable<KeyValuePair<ItemType, ScrollViewItemsView>> GetDeactivatedItemsViews(ItemType type) =>
      _shopItemsView.Where(item => item.Key != type);

    private bool IsUnlocked(string itemId) =>
      _gameSaveSystem.Get().UnlockedShopContent.Contains(itemId);

    private bool IsChosen(string itemId) =>
      _gameSaveSystem.Get().ChosenShopContent.Contains(itemId);
  }
}