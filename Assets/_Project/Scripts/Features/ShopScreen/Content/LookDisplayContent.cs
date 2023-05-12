using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.ShopScreen.Serialized;
using _Project.Scripts.Services._Interfaces;

namespace _Project.Scripts.Features.ShopScreen.Content
{
  public class LookDisplayContent : BaseDisplayContent
  {   
    private readonly SkinCombiner _skinCombiner;
    
    private readonly IGameSaveSystem _gameSaveSystem;
    
    private readonly UpgradeData[] _shopUpgrades;

    public LookDisplayContent(IGameSaveSystem gameSaveSystem, UpgradeData[] shopUpgrades)
    {
      _shopUpgrades = shopUpgrades;
      _gameSaveSystem = gameSaveSystem;
    }
    
    public override void Display(List<string> displayData)
    {
      UpgradeData microphone = _shopUpgrades.FirstOrDefault(FindSelectedMicrophone);
      UpgradeData skin = _shopUpgrades.FirstOrDefault(FindSelectedSkin);
      
      bool FindSelectedSkin(UpgradeData p) => 
        _gameSaveSystem.Get().ChosenUpgradeContent.Contains(p.Id) && p.ItemType == ShopItemType.Look;
      
      bool FindSelectedMicrophone(UpgradeData p) => 
        _gameSaveSystem.Get().ChosenUpgradeContent.Contains(p.Id) && p.ItemType == ShopItemType.Microphone;
      
      if (microphone != null && skin != null) 
        _skinCombiner.ChangeSkin(skin.DisplayData.FirstOrDefault(), microphone.DisplayData.FirstOrDefault());
    }

    public override void Hide(List<string> displayData)
    {}
  }
}