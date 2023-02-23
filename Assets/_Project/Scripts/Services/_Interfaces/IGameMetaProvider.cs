using System.Collections.Generic;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Services.GameMetaProvider;

namespace _Project.Scripts.Services._Interfaces
{
  public interface IGameMetaProvider
  {
    PlayerViewMono GetPlayerViewMono();
    void SetPlayerViewMono(PlayerViewMono playerViewMono);
    Dictionary<SymbolSign, PlayerData> GetPlayerData();
    
    void SetEnemyType(EnemyType enemyType);
    EnemyType GetEnemyType();
    
    void SetGame(Game game);
    Game GetGame();
    
    void SetGameMode(GameMode gameMode);
    GameMode GetGameMode();
  }
}