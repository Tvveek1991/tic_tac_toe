using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SableGamesAnalytics.Scripts.Data;
using SableGamesAnalytics.Scripts.Delayed;
using SableGamesAnalytics.Scripts.Inheritors;
using SableGamesAnalytics.Scripts.Interfaces;
using SableGamesAnalytics.Scripts.Logger;
using UnityEngine;

namespace SableGamesAnalytics
{
  public class Analytics : IAnalytics
  {
    private const string FirstLaunchKey = "Analytics.IsFirstLaunch";
    private const string AnalyticsClientIdKey = "Analytics.ClientId";

    private List<IAnalyticsModule> _modules;
    
    private readonly ICurrentScreen _currentScreen;
    private readonly ISessionManager _sessionManager;
    private readonly IAnalyticsLogger _logger;

    public event Action OnInitialize;
    private int _initializeCounter;

    private AnalyticsSettings _analyticsSettings;

    public string ClientId
    {
      get
      {
        if(!PlayerPrefs.HasKey(AnalyticsClientIdKey))
          PlayerPrefs.SetString(AnalyticsClientIdKey, GetUniqueClientId());
        return PlayerPrefs.GetString(AnalyticsClientIdKey);
      }
    }

    public bool IsFirstLaunch
    {
      get => PlayerPrefs.GetInt(FirstLaunchKey, 1) == 1;
      set => PlayerPrefs.SetInt(FirstLaunchKey, value ? 1 : 0);
    }

    public Analytics(ICurrentScreen currentScreen, ISessionManager sessionManager)
    {
      _logger = new AnalyticsLogger();
      
      _sessionManager = sessionManager;
      _currentScreen = currentScreen;
      
      LoadSettings();
    }

    public void Initialize()
    {
      _modules = new List<IAnalyticsModule>()
      {
        new FirebaseAnalytics(),
        new FlurryAnalytics(_analyticsSettings.FlurryKey, _logger),
#if !DISABLE_APPMETRICA_MYTRACKER
        new AppMetricaAnalytics(_analyticsSettings.AppmetricaKey),
        new MyTrackerAnalytics(_analyticsSettings.MyTrackerKey)
#endif
      };
      
      _modules.ForEach(p =>
      {
        p.OnInitialize += OnModuleInitialize;
        p.Initialize();
      });
    }

    public void Send(string s, Dictionary<string, object> parameters, bool withoutBaseParameters = false) => 
      SendInner(s, parameters, withoutBaseParameters);

    public void Send(string s, bool withoutBaseParameters = false)
    {
      var parameters = new Dictionary<string, object>();
      SendInner(s, parameters, withoutBaseParameters);
    }

    public bool ShouldSendSessionStart() =>
      _sessionManager.ShouldSendSessionStart();

    private void LoadSettings()
    {
#if UNITY_IOS
      _analyticsSettings = Resources.Load<AnalyticsSettings>("Sable Games Analytics Settings IOS");
#else
      _analyticsSettings = Resources.Load<AnalyticsSettings>("Sable Games Analytics Settings");
#endif

      if (_analyticsSettings == null) 
        _logger.ThrowNotSetupException();
      else
        _logger.SetSettings(_analyticsSettings);
    }

    protected virtual void CommonParameters(string eventName, ref Dictionary<string, object> parameters, bool withoutBaseParameters = false)
    {
      if(withoutBaseParameters)
        return;
      
      parameters.Add("client_id", ClientId);
      parameters.Add("count", PlayerPrefs.GetInt(eventName, 0));
      parameters.Add("session_number", _sessionManager.GetSessionNumber());
      parameters.Add("screen_name", _currentScreen.GetCurrentScreen());
    }

    private void SendInner(string s, Dictionary<string, object> parameters, bool withoutBaseParameters = false)
    {
      IncreaseEventCounter(s);
      CommonParameters(s, ref parameters, withoutBaseParameters);
      
      if(CheckNullValues(s, parameters))
        return;

      _modules.ForEach(p => p.Send(s, parameters));

      if (s == "ev_first_open")
        TimerForSend.SetTime(DateTime.UtcNow.AddHours(24));
      
      _logger.Log(s, parameters);
    }

    private void IncreaseEventCounter(string s) => 
      PlayerPrefs.SetInt(s, PlayerPrefs.GetInt(s, 0) + 1);

    private void OnModuleInitialize()
    {
      _initializeCounter++;
      
      if (_modules.Count != _initializeCounter)
        return;
      
      _modules.ForEach(p => p.OnInitialize -= OnModuleInitialize);
      OnInitialize?.Invoke();

      if (IsFirstLaunch) 
        SendFirstOpen();
      
      IsFirstLaunch = false;

      if(ShouldSendSessionStart())
        SendSessionStart();
      
      SendRetentionEvent();
    }

    private void SendFirstOpen() => 
      Send("ev_first_open");

    private void SendSessionStart() => 
      Send("ev_session_start");

    private void SendRetentionEvent()
    {
      TimerForSend.CheckoutTime(() => 
        Send("ev_retention"));
    }

    private bool CheckNullValues(string s, Dictionary<string, object> parameters)
    {
      if (!parameters.Values.Contains(null)) 
        return false;
      
      var stringBuilder = new StringBuilder();
      stringBuilder.AppendLine($"Analytics can't send null parameters - you must specify next parameters of {s}");

      var nullParameters = parameters.Where(parameter => parameter.Value == null).ToList();
      
      if(nullParameters.Count != 0)
        stringBuilder.AppendLine("");

      foreach (KeyValuePair<string, object> parameter in nullParameters)
        stringBuilder.AppendLine(parameter.Key);

      if (nullParameters.Any(p => p.Key == "screen_name"))
      {
        stringBuilder.AppendLine("");
        stringBuilder.AppendLine("Maybe, you should specify screen_name value in your ScreenManager script");
      }

      _logger.ThrowException<ArgumentException>(stringBuilder.ToString());

      return true;
    }

    private string GetUniqueClientId() => 
      Guid.NewGuid().ToString().Replace("-", "");
  }
}