using System;

namespace SableGamesAnalytics.Scripts.Interfaces
{
  public interface ICurrentScreen
  {
    event Action OnCurrentScreenSet;
    string GetCurrentScreen();
    void SetCurrentScreen(string name);
  }
}