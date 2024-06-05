using System.Collections.Generic;
using SableGamesRemote.Scripts.Data;
using UnityEngine;

namespace SableGamesAds.Scripts.Data
{
  public class SableGamesAdsSettings : ScriptableObject
  {
    public List<Configurations> PlacementsIOS;
    public List<Configurations> PlacementsAndroid;
    
    public bool EnableLimitations = true;
    
    public float TimeLimit = 30;
    public int ClicksLimit = 2;
    
    public bool IsLoggerEnabled;
    
    public static Dictionary<PlacementType, Dictionary<string, string>> Convert(SableGamesAdsSettings adsSettings)
    {
      var dictionary = new Dictionary<PlacementType, Dictionary<string, string>>();

#if UNITY_IOS
      var adsSettingsPlacements = adsSettings.PlacementsIOS;
#else
      var adsSettingsPlacements = adsSettings.PlacementsAndroid;
#endif
      
      foreach (Configurations adsSettingsPlacement in adsSettingsPlacements)
      {
        var placements = new Dictionary<string, string>();
        
        foreach (RemotePair remotePair in adsSettingsPlacement.RemotePair) 
          placements.Add(remotePair.BlockType, remotePair.Identifier);

        dictionary.Add(adsSettingsPlacement.PlacementType, placements);
      }
      
      return dictionary;
    }
  }
}
