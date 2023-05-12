using System.Collections.Generic;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.ShopScreen.Serialized;
using _Project.Scripts.Services._Interfaces;

namespace _Project.Scripts.Features.ShopScreen.Content
{
  public class ContentViewLogic : IContentViewLogic
  {
    private const string ShopItemsPath = "Features/Data/ShopData/Shop Items";

    private Dictionary<ShopItemType, BaseDisplayContent> _displayInstances;

    private IGameSaveSystem _gameSaveSystem;
    private IAssetProvider _assetProvider;
    
    private UpgradeData[] _shopUpgrades;

    public void Construct(IGameSaveSystem gameSaveSystem, IAssetProvider assetProvider)
    {
      _assetProvider = assetProvider;
      _gameSaveSystem = gameSaveSystem;

      _shopUpgrades = _assetProvider.GetAllResources<UpgradeData>(ShopItemsPath);

      var lookDisplayContent = new LookDisplayContent(_gameSaveSystem, _shopUpgrades);
      
      _displayInstances = new Dictionary<ShopItemType, BaseDisplayContent>()
      {
        {ShopItemType.Environment, new StageDisplayContent()},
        {ShopItemType.Look, lookDisplayContent},
        {ShopItemType.Microphone, lookDisplayContent},
      };
    }

    public void Display()
    {
      foreach (var upgrade in _shopUpgrades) 
        Hide(upgrade);

      foreach (var upgrade in _shopUpgrades) 
        if (_gameSaveSystem.Get().ChosenUpgradeContent.Contains(upgrade.Id))
          Display(upgrade);
    }

    private void Hide(UpgradeData upgradeData) => 
      _displayInstances[upgradeData.ItemType].Hide(upgradeData.DisplayData);

    private void Display(UpgradeData upgradeData) => 
      _displayInstances[upgradeData.ItemType].Display(upgradeData.DisplayData);
  }

  public abstract class BaseDisplayContent
  {
    public abstract void Display(List<string> displayData);
    public abstract void Hide(List<string> displayData);
  }
} 