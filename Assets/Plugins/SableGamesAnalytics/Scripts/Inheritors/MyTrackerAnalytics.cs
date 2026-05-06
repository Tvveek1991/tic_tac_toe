using System;
using System.Collections.Generic;
using System.Linq;
using Mycom.Tracker.Unity;
using SableGamesAnalytics.Scripts.Interfaces;
using UnityEngine;

namespace SableGamesAnalytics.Scripts.Inheritors
{
  public class MyTrackerAnalytics : IAnalyticsModule
  {
    private readonly string _key;
    public event Action OnInitialize;

    public MyTrackerAnalytics(string key) => 
      _key = key;

    public void Initialize()
    {
      if (Application.isEditor)
      {
        OnInitialize?.Invoke();
        return;
      }
      
#if !UNITY_IOS && !UNITY_ANDROID
        OnInitialize?.Invoke();   
        return;
#endif
      
      MyTracker.Init(_key);
      
      OnInitialize?.Invoke();   
      
      Debug.Log("[IAnalytics] MyTracker Initialized");
    }

    public void Send(string s, Dictionary<string, object> parameters)
    {
      Dictionary<string, string> newDictionary
        = parameters.ToDictionary(item => item.Key, item => item.Value.ToString());

      if(Application.isEditor)
      {
        Debug.Log("MyTracker Stub");
        return;
      }      
      
      MyTracker.TrackEvent(s, newDictionary);
    }


    public void Send(string s)
    { 
      if(Application.isEditor)
      {
        Debug.Log("MyTracker Stub");
        return;
      }      
      
      MyTracker.TrackEvent(s);
    }
  }
}