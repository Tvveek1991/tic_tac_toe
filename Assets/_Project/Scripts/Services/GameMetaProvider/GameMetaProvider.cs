using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Services.GameMetaProvider
{
  public class GameMetaProvider : IGameMetaProvider
  {
    private readonly IAssetProvider _assetProvider;
    private readonly IGameSaveSystem _gameSaveSystem;

    private const string CustomCrossDataPath = "Data/Symbols/X";
    private const string CustomZeroDataPath = "Data/Symbols/O";

    private BasePlayerViewMono BasePlayerViewMono { get; set; }

    private List<SymbolData> SymbolDataX { get; set; }
    private List<SymbolData> SymbolDataO { get; set; }

    private Dictionary<SymbolSign, PlayerData> _playerData;

    public GameMetaProvider(IAssetProvider assetProvider, IGameSaveSystem gameSaveSystem)
    {
      _assetProvider = assetProvider;
      _gameSaveSystem = gameSaveSystem;

      PreloadData();
    }

    public Dictionary<SymbolSign, PlayerData> GetPlayerData() =>
      _playerData;

    public void SetPlayerViewMono(BasePlayerViewMono basePlayerViewMono) => 
      BasePlayerViewMono = basePlayerViewMono;

    public BasePlayerViewMono GetPlayerViewMono() =>
      BasePlayerViewMono;

    public void SetGame(Game game)
    {
      _gameSaveSystem.Get().Game = game;
      _gameSaveSystem.Save();
    }

    public Game GetGame() =>
      _gameSaveSystem.Get().Game;

    public void SetEnemyType(EnemyType enemyType)
    {
      _gameSaveSystem.Get().Enemy = enemyType;
      _gameSaveSystem.Save();
    }

    public EnemyType GetEnemyType() =>
      _gameSaveSystem.Get().Enemy;

    public void SetGameMode(GameMode gameMode)
    {
      _gameSaveSystem.Get().Mode = gameMode;
      _gameSaveSystem.Save();
    }

    public GameMode GetGameMode() =>
      _gameSaveSystem.Get().Mode;

    public void Clear()
    {
      if (BasePlayerViewMono)
        Object.Destroy(BasePlayerViewMono.gameObject);
    }

    private void PreloadData()
    {
      SymbolDataX = Load<SymbolData>(CustomCrossDataPath);
      SymbolDataO = Load<SymbolData>(CustomZeroDataPath);

      if (_gameSaveSystem.Get().IsFirstSession)
      {
        _gameSaveSystem.Get().ChosenModelX = SymbolDataX[0].Id;
        _gameSaveSystem.Get().ChosenModelO = SymbolDataO[0].Id;
      }

      CreatePlayerData();
    }

    private void CreatePlayerData()
    {
      _playerData = new Dictionary<SymbolSign, PlayerData>
      {
        {
          SymbolSign.X, new PlayerData()
          {
            Sign = SymbolSign.X,
            Color = Color.red,
            SymbolData = SymbolDataX.FirstOrDefault(item => item.Id == _gameSaveSystem.Get().ChosenModelX)
          }
        },
        {
          SymbolSign.O, new PlayerData()
          {
            Sign = SymbolSign.O,
            Color = Color.blue,
            SymbolData = SymbolDataO.FirstOrDefault(item => item.Id == _gameSaveSystem.Get().ChosenModelO)
          }
        }
      };
    }

    public Mesh GetMesh(DragableData dragableData) =>
      GetGame() == Game.Classic
        ? GetPlayerData()[dragableData.GetSymbol()].SymbolData.MeshClassic
        : GetPlayerData()[dragableData.GetSymbol()].SymbolData.MeshBuhurt[(int) dragableData.GetSize()];

    public Material GetMaterial(DragableData dragableData) =>
      GetGame() == Game.Classic
        ? GetPlayerData()[dragableData.GetSymbol()].SymbolData.MaterialClassic
        : GetPlayerData()[dragableData.GetSymbol()].SymbolData.MaterialBuhurt[(int) dragableData.GetSize()];

    private List<SymbolData> Load<T>(string path) =>
      _assetProvider.GetAllResources<SymbolData>(path).ToList();
  }

  [Serializable]
  public class PlayerData
  {
    public SymbolSign Sign;
    public Color Color;
    public SymbolData SymbolData;
  }
}