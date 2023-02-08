using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.GameScreen.Serialize;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Services.GameMetaProvider
{
  public class GameMetaProvider : IGameMetaProvider
  {
    private readonly IAssetProvider _assetProvider;
    private readonly IGameSaveSystem _gameSaveSystem;

    private const string CustomCrossDataPath = "Data/Symbols/X";
    private const string CustomZeroDataPath = "Data/Symbols/O";
    
    private PlayerViewMono PlayerViewMono { get; set; }
    
    private List<SymbolData> SymbolDataX { get; set; }
    private List<SymbolData> SymbolDataO { get; set; }

    private Dictionary<SymbolSign, PlayerData> _playerData;

    public GameMetaProvider(IAssetProvider assetProvider, IGameSaveSystem gameSaveSystem)
    {
      _assetProvider = assetProvider;
      _gameSaveSystem = gameSaveSystem;

      PreloadData();
    }

    public void SetPlayerViewMono(PlayerViewMono playerViewMono) => 
      PlayerViewMono = playerViewMono;

    public PlayerViewMono GetPlayerViewMono() =>
      PlayerViewMono;

    public Dictionary<SymbolSign, PlayerData> GetPlayerData() =>
      _playerData;

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
            Color = Color.red,
            SymbolData = SymbolDataX.FirstOrDefault(item => item.Id == _gameSaveSystem.Get().ChosenModelX)
          }
        },
        {
          SymbolSign.O, new PlayerData()
          {
            Color = Color.blue,
            SymbolData = SymbolDataO.FirstOrDefault(item => item.Id == _gameSaveSystem.Get().ChosenModelO)
          }
        }
      };
    }

    private List<SymbolData> Load<T>(string path) =>
      _assetProvider.GetAllResources<SymbolData>(path).ToList();
  }

  [Serializable]
  public class PlayerData
  {
    public Color Color;
    public SymbolData SymbolData;
  }
}