using SableGamesAnalytics.Scripts.Interfaces;
using UnityEngine;

namespace SableGamesAnalytics.Scripts.Logic
{
  public class UpgradeManager : IUpgradeManager
  {
    private const string UpgradeCountKey = "UpgradeCount";

    private int UpgradeCount
    {
      get => PlayerPrefs.GetInt(UpgradeCountKey, 0);
      set => PlayerPrefs.SetInt(UpgradeCountKey, value);
    }
    
    public int GetUpgradesCount() => 
      UpgradeCount;

    public void IncrementUpgradeCount()
    {
      UpgradeCount++;
      Debug.Log($"[IUpgradeManager] IncrementUpgradeCount To {UpgradeCount}");
    }
  }
}