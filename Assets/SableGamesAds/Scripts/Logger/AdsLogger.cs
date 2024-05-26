using System.Text;
using SableGamesAds.Scripts.Data;
using UnityEngine;

namespace SableGamesAds.Scripts.Logger
{
  public class AdsLogger : IAdsLogger
  {
    private SableGamesAdsSettings _adsSettings;

    public void SetSettings(SableGamesAdsSettings adsSettings) => 
      _adsSettings = adsSettings;

    public void ThrowNotSetupException()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("File not found - Sable Games Ads Settings.asset");
      stringBuilder.AppendLine("Please, setup through Sable Games/Create Ads Settings menu");
      throw new SableGamesAdsException(stringBuilder.ToString());
    }

    public void LogWithoutFormat(string s)
    {     
      if(!_adsSettings.IsLoggerEnabled)
        return;
      
      Debug.Log($"[IAds] {s}");
    }
  }
}