using System;
using System.Collections.Generic;
using SableGamesAnalytics.Scripts.Interfaces;
using UnityEngine;

namespace SableGamesAnalytics
{
  public class AnalyticsDummy : IAnalytics
  {
    public bool IsFirstLaunch { get; set; }
    public event Action OnInitialize;

    public void Initialize() => 
      OnInitialize?.Invoke();

    public void Send(string s, Dictionary<string, object> parameters, bool withoutBaseParameters = false)
    { Debug.LogWarning("[IAnalytics] You are using Dummy class now"); }

    public void Send(string s, bool withoutBaseParameters = false)
    { Debug.LogWarning("[IAnalytics] You are using Dummy class now"); }
    
    public bool ShouldSendSessionStart() => 
      false;
  }
}