using System.Collections.Generic;
using _Project.Scripts.Features.GameScreen.Serialize;

namespace _Project.Scripts.Services._data
{
  public class GameSaveData
  {
    public bool IsFirstSession = true;

    public bool IsPolicyAccepted;

    public bool IsMusicDisabled;
    public bool IsSoundDisabled;
    public bool IsVibrationDisabled;

    public uint MoneyCount = 100;

    public string ChosenModelX;
    public string ChosenModelO;
    
    public string ChosenColorX;
    public string ChosenColorO;
  }
}