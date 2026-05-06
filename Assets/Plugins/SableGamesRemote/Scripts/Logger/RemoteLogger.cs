using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RemoteSettings = SableGamesRemote.Scripts.Data.RemoteSettings;

namespace SableGamesRemote.Scripts.Logger
{
  public class RemoteLogger : IRemoteLogger
  {
    private RemoteSettings _remoteSettings;

    public void SetSettings(RemoteSettings analyticsSettings) => 
      _remoteSettings = analyticsSettings;

    public void ThrowNotSetupException()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("File not found - Sable Games Remote Settings.asset");
      stringBuilder.AppendLine("Please, setup through Sable Games/Create Remote Settings menu");
      throw new SableGamesRemoteException(stringBuilder.ToString());
    }
    
    public void Log(string s, Dictionary<string, object> parameters)
    {
      if(!_remoteSettings.IsLoggerEnabled)
        return;
      
      StringBuilder stringBuilder = new StringBuilder();

      foreach (KeyValuePair<string, object> item in parameters)
        stringBuilder.AppendLine($"{item.Key} : {item.Value}");
      
      Debug.Log($"[IRemote] Send: {s} params {stringBuilder}");
    }
    
    public void LogWithoutFormat(string s)
    {     
      if(!_remoteSettings.IsLoggerEnabled)
        return;
      
      Debug.Log($"[IRemote] {s}");
    }
  }
}