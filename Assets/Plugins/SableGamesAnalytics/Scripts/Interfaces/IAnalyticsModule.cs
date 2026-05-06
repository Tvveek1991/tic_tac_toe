using System;
using System.Collections.Generic;

namespace SableGamesAnalytics.Scripts.Interfaces
{
  public interface IAnalyticsModule
  {
    void Initialize();
    void Send(string s, Dictionary<string, object> parameters);
    event Action OnInitialize;
  }
}