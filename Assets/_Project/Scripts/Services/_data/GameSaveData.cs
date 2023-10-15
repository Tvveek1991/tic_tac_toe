using System.Collections.Generic;
using _Project.Scripts.Features.GameScreen.Serialized;

namespace _Project.Scripts.Services._data
{
  public class GameSaveData
  {
    public bool IsFirstBuhurtSession;

    public bool IsPolicyAccepted;

    public bool IsDisableAdsFirstTime = true;
    public bool IsDisableAdsActive;
    public int DisableAdsProgress;

    public bool DisableAdsIsPurchased;
    
    public bool IsMusicDisabled;
    public bool IsSoundDisabled;
    public bool IsVibrationDisabled;

    public uint WinStreak;
    public uint MoneyCount = 100;
    
    public List<string> ChosenShopContent = new ();
    public List<string> UnlockedShopContent = new ();


    public Game Game = Game.Classic;
    public EnemyType Enemy = EnemyType.Bot;
  }
}