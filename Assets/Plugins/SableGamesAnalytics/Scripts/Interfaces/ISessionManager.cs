namespace SableGamesAnalytics.Scripts.Interfaces
{
  public interface ISessionManager
  {
    int GetSessionNumber();
    bool ShouldSendSessionStart();
  }
}