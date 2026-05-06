using System;
using SableGamesAnalytics.Scripts.Interfaces;

namespace SableGamesAnalytics.Scripts.Logic
{
  public class ScreenManager : ICurrentScreen
  {
    private string _sceneName /*= SceneManager.GetActiveScene().name*/;
    
    public event Action OnCurrentScreenSet;
    
    public string GetCurrentScreen() => 
      _sceneName;

    public void SetCurrentScreen(string name)
    {
      _sceneName = name;
      OnCurrentScreenSet?.Invoke();
    }
  }
}