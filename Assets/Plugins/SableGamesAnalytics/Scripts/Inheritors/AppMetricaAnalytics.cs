using System;
using System.Collections.Generic;
using SableGamesAnalytics.Scripts.Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SableGamesAnalytics.Scripts.Inheritors
{
  public class AppMetricaAnalytics : IAnalyticsModule
  {
    private const string AppmetricaPath = "Appmetrica";
    
    private AppMetrica _appMetrica;
    private readonly string _key;

    public event Action OnInitialize;

    public AppMetricaAnalytics(string key) => 
      _key = key;

    public void Initialize()
    {
      var appMetrica = Resources.Load<GameObject>(AppmetricaPath);
      _appMetrica = Object.Instantiate(appMetrica).GetComponent<AppMetrica>();
      _appMetrica.SetId(_key);
      
      _appMetrica.Construct();
      _appMetrica.Launch();
      
      OnInitialize?.Invoke();
      
      Debug.Log("[IAnalytics] AppMetrica Initialized");
    }

    public void Send(string s, Dictionary<string, object> parameters) =>
      AppMetrica.Instance.ReportEvent(s, parameters);

    public void Send(string s) => 
      AppMetrica.Instance.ReportEvent(s);
  }
}