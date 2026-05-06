using SableGamesRemote.Scripts.Data;

namespace SableGamesRemote.Scripts.Logger
{
  public interface IRemoteLogger
  {
    void ThrowNotSetupException(); 
    void SetSettings(RemoteSettings remoteSettings);
    void LogWithoutFormat(string s);
  }
}