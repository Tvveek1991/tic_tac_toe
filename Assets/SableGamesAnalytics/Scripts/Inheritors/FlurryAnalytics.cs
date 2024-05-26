using System;
using System.Collections.Generic;
using System.Linq;
using FlurrySDK;
using SableGamesAnalytics.Scripts.Interfaces;
using SableGamesAnalytics.Scripts.Logger;
using UnityEngine;

namespace SableGamesAnalytics.Scripts.Inheritors
{
  public class FlurryAnalytics : IAnalyticsModule
  {
    private readonly string _flurryAPIKey;
    
    private static readonly List<Event> DelayedAnalyticsEvents = new List<Event>();
    
    private readonly IAnalyticsLogger _analyticsLogger;

    public FlurryAnalytics(string analyticsSettingsFlurryKey, IAnalyticsLogger analyticsLogger)
    {
      _analyticsLogger = analyticsLogger;
      _flurryAPIKey = analyticsSettingsFlurryKey;
    }

    public event Action OnInitialize;

    public void Initialize()
    {
      new Flurry.Builder()
        .WithCrashReporting()
        .WithLogEnabled()
        .WithLogLevel(Flurry.LogLevel.VERBOSE)
        .WithMessaging()
        .Build(_flurryAPIKey);

      _analyticsLogger.LogWithoutFormat("AgentVersion: " + Flurry.GetAgentVersion());
      _analyticsLogger.LogWithoutFormat("ReleaseVersion: " + Flurry.GetReleaseVersion());

      Flurry.SetAge(36);
      Flurry.SetGender(Flurry.Gender.Female);
      Flurry.SetReportLocation(true);

      Flurry.UserProperties.Add(Flurry.UserProperties.PROPERTY_REGISTERED_USER, "True");

      Flurry.SetMessagingListener(new MyMessagingListener());

      Flurry.EventRecordStatus status = Flurry.LogEvent("Unity Event");

      IDictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("Author", "Flurry");
      parameters.Add("Status", "Registered");
      status = Flurry.LogEvent("Unity Event Params Timed", parameters, true);

      Flurry.EndTimedEvent("Unity Event Params Timed");

      foreach (Event item in DelayedAnalyticsEvents)
      {
        if (item.Parameters == null)
          Flurry.LogEvent(item.s);
        else
          Flurry.LogEvent(item.s, item.Parameters);
      }

      DelayedAnalyticsEvents.Clear();
      
      OnInitialize?.Invoke();
      
      Debug.Log("[IAnalytics] Flurry Initialized");
    }

    public void Send(string s, Dictionary<string, object> parameters)
    {
      Dictionary<string, string> updatedParameters = parameters.ToDictionary(item =>
        item.Key, item
        => item.Value.ToString());

      try
      {
        Flurry.LogEvent(s, updatedParameters);
      }
      catch (NullReferenceException e)
      {
        DelayedAnalyticsEvents.Add(new Event() {s = s, Parameters = updatedParameters});
      }
    }

    [Serializable]
    public struct Event
    {
      public string s;
      public Dictionary<string, string> Parameters;
    }
  }
}