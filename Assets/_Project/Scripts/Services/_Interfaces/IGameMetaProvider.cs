using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Services.Audio.Enum;
using UnityEngine;

namespace _Project.Scripts.Services._Interfaces
{
  public interface IGameMetaProvider
  {
    BasePlayerViewMono GetPlayerView();
    void SetPlayerViewMono(BasePlayerViewMono basePlayerViewMono);
    void Clear();
    
    void SetEnemyType(EnemyType enemyType);
    EnemyType GetEnemyType();
    
    void SetGame(Game game);
    Game GetGame();

    Mesh GetMesh(DragableData dragableData);
    Material GetMaterial(DragableData dragableData);
    Vector3 GetAngle(DragableData dragableData);
    Sprite GetIcon(SymbolSign symbol);
    
    AudioId GetMusic();
    MapData GetMap();

    void AddScore(SymbolSign symbolSign);
    int GetScore(SymbolSign symbolSign);
    void ClearScore();

    void CreateBotData(SymbolSign botSign);

    void AddWinStreak(bool isWin);
    uint GetWinStreak(bool isBest);
  }
}