using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Services.Audio.Enum;
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

    Mesh GetMesh(SymbolSign symbolSign, SymbolSize symbolSize);
    Material GetMaterial(SymbolSign symbolSign, SymbolSize symbolSize);
    Sprite GetIcon(SymbolSign symbol);
    
    AudioId GetMusic();
    MapData GetMap();

    void SetOpponentData(SymbolSign symbolSign);
  }
}