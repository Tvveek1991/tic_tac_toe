using UnityEngine;

namespace SableGamesAnalytics.Scripts.Data
{
  public class AnalyticsSettings : ScriptableObject
  {
    public string FlurryKey;
    public string AppmetricaKey;
    public string MyTrackerKey;

    public bool IsLoggerEnabled;
  }
}