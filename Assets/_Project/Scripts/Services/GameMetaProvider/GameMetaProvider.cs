using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Features.ShopScreen.Serialized;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio.Enum;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Services.GameMetaProvider
{
  public class GameMetaProvider : IGameMetaProvider
  {
    private const string GameDataPath = "Data/Game";

    private readonly IAssetProvider _assetProvider;
    private readonly IGameSaveSystem _gameSaveSystem;

    private readonly GameCustomData[] _gameCustomsData;

    private readonly Dictionary<SymbolSign, ItemType> _itemTypesTransfer = new()
    {
      {SymbolSign.X, ItemType.SymbolX},
      {SymbolSign.O, ItemType.SymbolO}
    };

    private uint _winStreak;
    private Dictionary<SymbolSign, int> _playersScore;
    private readonly Dictionary<SymbolSign, SymbolData> _botData = new() {{SymbolSign.X, null}, {SymbolSign.O, null}};

    private BasePlayerViewMono BasePlayerViewMono { get; set; }

    public GameMetaProvider(IAssetProvider assetProvider, IGameSaveSystem gameSaveSystem)
    {
      _assetProvider = assetProvider;
      _gameSaveSystem = gameSaveSystem;

      _gameCustomsData = Load<GameCustomData>(GameDataPath);
    }

    public void SetPlayerViewMono(BasePlayerViewMono basePlayerViewMono) =>
      BasePlayerViewMono = basePlayerViewMono;

    public BasePlayerViewMono GetPlayerView() =>
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

    public Sprite GetIcon(SymbolSign sign)
    {
      ItemType itemType = _itemTypesTransfer[sign];
      return GetCustomData<SymbolData>(itemType).Icon;
    }

    public AudioId GetMusic() =>
      GetCustomData<MusicData>(ItemType.Music).ClipId;

    public MapData GetMap() =>
      GetCustomData<MapData>(ItemType.Field);

    public Mesh GetMesh(SymbolSign symbolSign, SymbolSize symbolSize)
    {
      SymbolData symbolData = GetSymbolData(symbolSign);
      return GetGame() == Game.Classic
        ? symbolData.MeshClassic
        : symbolData.MeshBuhurt[(int) symbolSize];
    }

    public Material GetMaterial(SymbolSign symbolSign, SymbolSize symbolSize)
    {
      SymbolData symbolData = GetSymbolData(symbolSign);
      return GetGame() == Game.Classic
        ? symbolData.MaterialClassic
        : symbolData.MaterialBuhurt[(int) symbolSize];
    }

    public void AddScore(SymbolSign symbolSign)
    {
      _playersScore ??= new Dictionary<SymbolSign, int>() {{SymbolSign.None, 0}, {SymbolSign.X, 0}, {SymbolSign.O, 0}};
      _playersScore[symbolSign]++;
    }

    public int GetScore(SymbolSign symbolSign) =>
      _playersScore[symbolSign];

    public void ClearScore()
    {
      _winStreak = 0;
      _playersScore = null;
    }

    public void AddWinStreak(bool isWin)
    {
      _winStreak = isWin ? ++_winStreak : 0;

      if (_winStreak <= _gameSaveSystem.Get().WinStreak)
        return;

      _gameSaveSystem.Get().WinStreak = _winStreak;
      _gameSaveSystem.Save();
    }

    public uint GetWinStreak(bool isBest) =>
      isBest ? _gameSaveSystem.Get().WinStreak : _winStreak;

    public void Clear()
    {
      if (BasePlayerViewMono)
        Object.Destroy(BasePlayerViewMono.gameObject);
    }

    public void CreateBotData(SymbolSign botSign)
    {
      List < SymbolData > list = GetAllDataType<SymbolData>(_gameCustomsData).Where(item => item.ItemType == _itemTypesTransfer[botSign]).ToList();
      _botData[botSign] = list[Random.Range(0, list.Count)];
    }

    private SymbolData GetSymbolData(SymbolSign symbolSign) =>
      !BasePlayerViewMono.GetPlayerTurn() && GetEnemyType() == EnemyType.Bot
        ? _botData[symbolSign]
        : GetCustomData<SymbolData>(_itemTypesTransfer[symbolSign]);

    private T[] Load<T>(string path) where T : BaseContentData =>
      _assetProvider.GetAllResources<T>(path);

    private T GetCustomData<T>(ItemType itemType) where T : GameCustomData
    {
      foreach (T data in GetAllDataType<T>(_gameCustomsData))
      {
        if (!_gameSaveSystem.Get().ChosenShopContent.Contains(data.Id) || data.ItemType != itemType)
          continue;
        return data;
      }

      Debug.LogException(new Exception("Didn't find any chosen data with type " + itemType));
      return null;
    }

    private T[] GetAllDataType<T>(GameCustomData[] gameCustomData) where T : GameCustomData
    {
      var filteredData = new List<T>();

      foreach (GameCustomData data in gameCustomData)
        if (data is T symbolData)
          filteredData.Add(symbolData);

      return filteredData.ToArray();
    }
  }
}