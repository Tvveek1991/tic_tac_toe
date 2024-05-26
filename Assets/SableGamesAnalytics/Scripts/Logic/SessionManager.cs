using SableGamesAnalytics.Scripts.Interfaces;
using SableGamesAnalytics.Scripts.Mono;
using UnityEngine;

namespace SableGamesAnalytics.Scripts.Logic
{
  public class SessionManager : ISessionManager
  {
    private readonly SessionManagerHandler _handler;

    public SessionManager()
    {
      _handler = new GameObject("Session Handler")
        .AddComponent<SessionManagerHandler>();

      _handler.Launch();
    }

    public int GetSessionNumber() => 
      _handler.SessionId;
    
    public bool ShouldSendSessionStart() => 
      _handler.ShouldSendSessionStart();
  }
}