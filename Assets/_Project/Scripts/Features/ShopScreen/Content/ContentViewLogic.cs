using System.Collections.Generic;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.ShopScreen.Serialized;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.ShopScreen.Content
{
  public class ContentViewLogic : IContentViewLogic
  {
    private const string ShopItemsPath = "Data/Shop/Shop Items";

    private Dictionary<ItemType, BaseDisplayContent> _displayInstances;

    private IGameSaveSystem _gameSaveSystem;
    private IAssetProvider _assetProvider;

    private ShopItemData[] _shopItemsData;

    public void Construct(IGameMetaProvider gameMetaProvider, IGameSaveSystem gameSaveSystem, IAssetProvider assetProvider, IAudioService audioService)
    {
      _assetProvider = assetProvider;
      _gameSaveSystem = gameSaveSystem;

      _shopItemsData = _assetProvider.GetAllResources<ShopItemData>(ShopItemsPath);
      var lookDisplayContent = new LookDisplayContent(gameMetaProvider, _gameSaveSystem, audioService);

      _displayInstances = new Dictionary<ItemType, BaseDisplayContent>()
      {
        {ItemType.SymbolX, lookDisplayContent},
        {ItemType.SymbolO, lookDisplayContent},
        {ItemType.Field, lookDisplayContent},
        {ItemType.Music, lookDisplayContent},
      };
    }

    public void Display()
    {
      foreach (ShopItemData upgrade in _shopItemsData)
        if (_gameSaveSystem.Get().ChosenShopContent.Contains(upgrade.Id))
          Display(upgrade);
    }

    private void Display(ShopItemData shopItemData) => 
      _displayInstances[shopItemData.ItemType].Display(shopItemData);
  }

  public abstract class BaseDisplayContent
  {
    public abstract void Display(ShopItemData shopItemData);
  }
}