using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Features.Player;
using UnityEngine;

namespace _Project.Scripts.Services._Interfaces
{
  public interface IGameMetaProvider
  {
    BasePlayerViewMono GetPlayerViewMono();
    void SetPlayerViewMono(BasePlayerViewMono basePlayerViewMono);
    void Clear();
    
    void SetEnemyType(EnemyType enemyType);
    EnemyType GetEnemyType();
    
    void SetGame(Game game);
    Game GetGame();
    
    void SetGameMode(GameMode gameMode);
    GameMode GetGameMode();

    Mesh GetMesh(SymbolSign symbolSign, SymbolSize symbolSize);
    Material GetMaterial(SymbolSign symbolSign, SymbolSize symbolSize);
    Sprite GetIcon(SymbolSign symbol);
  }
}