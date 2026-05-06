using System;
using System.Collections.Generic;
using Common;

namespace SableGamesAnalytics.Scripts.Interfaces
{
  public interface IAnalytics : IService
  {
    void Initialize();
    void Send(string s, Dictionary<string, object> parameters, bool withoutBaseParameters = false);
    void Send(string s, bool withoutBaseParameters = false);
    bool ShouldSendSessionStart();
    bool IsFirstLaunch { get; set; }
    event Action OnInitialize;
  }
}