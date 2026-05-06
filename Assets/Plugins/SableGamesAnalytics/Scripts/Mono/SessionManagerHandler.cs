using System;
using System.Globalization;
using UnityEngine;

namespace SableGamesAnalytics.Scripts.Mono
{
  public class SessionManagerHandler : Singleton<SessionManagerHandler> 
  {
    private const string SessionTimer = "Updated.Session_Timer";
    private const string Session = "Updated.Session";

    private const int SessionPeriod = 5;

    public int SessionId
    {
      get => PlayerPrefs.GetInt(Session, 0);
      private set => PlayerPrefs.SetInt(Session, value);
    }

    private static bool _isSessionChecked;
    
    private bool _isIncremented;

    public void Launch() => 
      IncrementAndSend();

    public bool ShouldSendSessionStart() 
      => _isIncremented;

    private void OnApplicationQuit() => 
      SaveTime();

    private void OnApplicationPause(bool pause)
    {
      if (pause)
      {
        if (_isSessionChecked) 
          SaveTime();
      }
    }

    private void IncrementAndSend()
    {
      if (PlayerPrefs.HasKey(SessionTimer))
      {
        string s = PlayerPrefs.GetString(SessionTimer, DateTime.MinValue.ToString(CultureInfo.InvariantCulture));
        DateTime dateSaved = DateTime.Parse(s, CultureInfo.InvariantCulture);
        
        if ((DateTime.Now - dateSaved).TotalMinutes > SessionPeriod)
          SessionUp();
        else
          PlayerPrefs.SetString(SessionTimer, DateTime.Now.ToString(CultureInfo.InvariantCulture));
      }
      else
      {
        SessionUp();
      }

      _isSessionChecked = true;
    }

    private void SessionUp()
    {
      SessionId++;
      _isIncremented = true;
    }

    private void SaveTime() =>
      PlayerPrefs.SetString(SessionTimer, DateTime.Now.ToString(CultureInfo.InvariantCulture));
  }
}