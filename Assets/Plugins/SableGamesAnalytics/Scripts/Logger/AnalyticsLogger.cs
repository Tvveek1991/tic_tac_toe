using System;
using System.Collections.Generic;
using System.Text;
using SableGamesAnalytics.Scripts.Data;
using UnityEngine;

namespace SableGamesAnalytics.Scripts.Logger
{
  public class AnalyticsLogger : IAnalyticsLogger
  {
    private AnalyticsSettings _analyticsSettings;

    public void SetSettings(AnalyticsSettings analyticsSettings) => 
      _analyticsSettings = analyticsSettings;

    public void ThrowNotSetupException()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("File not found - Sable Games Analytics Settings.asset");
      stringBuilder.AppendLine("Please, setup through Sable Games/Create Analytics Settings menu");
      throw new SableGamesAnalyticsException(stringBuilder.ToString());
    }
    
    public void Log(string s, Dictionary<string, object> parameters)
    {
      if(!_analyticsSettings.IsLoggerEnabled)
        return;
      
      StringBuilder stringBuilder = new StringBuilder();

      foreach (KeyValuePair<string, object> item in parameters)
        stringBuilder.Append($"{item.Key} : {item.Value}, ");

      object message = $"[IAnalytics] Send Event: {s} Parameters: [{stringBuilder}]";
      
      Debug.unityLogger.Log("Analytics", message);
    }
    
    public void LogWithoutFormat(string s)
    {     
      if(!_analyticsSettings.IsLoggerEnabled)
        return;
      
      Debug.Log($"[IAnalytics] {s}");
    }

    public void ThrowException<T>(string s) where T : Exception => 
      throw ((T) Activator.CreateInstance(typeof(T), s));
  }
}