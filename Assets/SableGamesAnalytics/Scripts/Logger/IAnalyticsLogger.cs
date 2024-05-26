using System;
using System.Collections.Generic;
using SableGamesAnalytics.Scripts.Data;

namespace SableGamesAnalytics.Scripts.Logger
{
  public interface IAnalyticsLogger
  {
    void ThrowNotSetupException();
    void Log(string s, Dictionary<string, object> parameters);
    void SetSettings(AnalyticsSettings analyticsSettings);
    void LogWithoutFormat(string s);
    void ThrowException<T>(string s) where T : Exception;
  }
}