using System;
using System.Collections.Generic;
using Firebase.Analytics;
using SableGamesAnalytics.Scripts.Interfaces;
using UnityEngine;

namespace SableGamesAnalytics.Scripts.Inheritors
{
  public class FirebaseAnalytics : IAnalyticsModule
  {
    public event Action OnInitialize;

    public void Initialize()
    {
      OnInitialize?.Invoke();
      Debug.Log("[IAnalytics] Firebase Initialized");
    }

    public void Send(string s, Dictionary<string, object> parameters)
    {
      var castParameters = new List<Parameter>();
      
      foreach (var item in parameters)
      {
        switch (item.Value)
        {
          case string value:
            castParameters.Add(new Parameter(item.Key, value));
            break;
          case double value:
            castParameters.Add(new Parameter(item.Key, value));
            break;
          case float value:
            castParameters.Add(new Parameter(item.Key, value));
            break;
          case long value:
            castParameters.Add(new Parameter(item.Key, value));
            break;
          case int value:
            castParameters.Add(new Parameter(item.Key, value));
            break;
          default:
            castParameters.Add(new Parameter(item.Key, item.Value.ToString()));
            break;
        }
      }

      Firebase.Analytics.FirebaseAnalytics.LogEvent(s, castParameters.ToArray());
    }
  }
}