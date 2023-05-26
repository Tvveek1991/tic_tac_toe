using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Features.ShopScreen.Serialized;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Unlocker;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Services.GameMetaProvider
{
  public class GameMetaProvider : IGameMetaProvider
  {
    private const string GameDataPath = "Data/Game";

    private readonly IAssetProvider _assetProvider;
    private readonly IUnlockService _unlockService;
    private readonly IGameSaveSystem _gameSaveSystem;

    private readonly GameCustomData[] _gameCustomsData;

    private int _opponentSkinIndex;
    private BasePlayerViewMono BasePlayerViewMono { get; set; }
    
    private readonly Dictionary<SymbolSign, ItemType> _itemTypesTransfer = new ()
    {
      {SymbolSign.X, ItemType.SymbolX},
      {SymbolSign.O, ItemType.SymbolO}
    };

    public GameMetaProvider(IAssetProvider assetProvider, IGameSaveSystem gameSaveSystem, IUnlockService unlockService)
    {
      _assetProvider = assetProvider;
      _unlockService = unlockService;
      _gameSaveSystem = gameSaveSystem;

      _gameCustomsData = Load<GameCustomData>(GameDataPath);

      if (_gameSaveSystem.Get().IsFirstSession)
        OpenFree();
    }

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

    public Sprite GetIcon(SymbolSign sign)
    {
      ItemType itemType = _itemTypesTransfer[sign];
      return GetCustomData<SymbolData>(itemType).Icon;
    }
    
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

    public int GetSkinIndex(SymbolSign symbol)
    {
      List<SymbolData> list = GetAllDataType<SymbolData>(_gameCustomsData).Where(item => item.ItemType == _itemTypesTransfer[symbol]).ToList();
      SymbolData data = list.FirstOrDefault(item => _gameSaveSystem.Get().ChosenShopContent.Contains(item.Id));
      
      return list.IndexOf(data);
    }

    public void SetOpponentSkinIndex(int skinIndex)
    {
      _opponentSkinIndex = skinIndex;
      Debug.Log(skinIndex);
    }

    public void Clear()
    {
      if (BasePlayerViewMono)
        Object.Destroy(BasePlayerViewMono.gameObject);
    }

    private SymbolData GetSymbolData(SymbolSign symbolSign) =>
      BasePlayerViewMono.GetPlayerTurn() 
        ? GetCustomData<SymbolData>(_itemTypesTransfer[symbolSign]) 
        : GetCustomOpponentData<SymbolData>(_itemTypesTransfer[symbolSign]);

    private void OpenFree()
    {
      foreach (GameCustomData gameCustomData in _gameCustomsData)
      {
        if (IsUnlocked(gameCustomData.Id) || _unlockService.GetUnlockType(gameCustomData.Id) != UnlockType.Free)
          continue;

        _gameSaveSystem.Get().UnlockedShopContent.Add(gameCustomData.Id);
        _gameSaveSystem.Get().ChosenShopContent.Add(gameCustomData.Id);
      }

      _gameSaveSystem.Get().IsFirstSession = false;
      _gameSaveSystem.Save();
    }

    private bool IsUnlocked(string itemId) =>
      _gameSaveSystem.Get().UnlockedShopContent.Contains(itemId);

    private T[] Load<T>(string path) where T : BaseContentData =>
      _assetProvider.GetAllResources<T>(path);

    private T GetCustomData<T>(ItemType itemType) where T : GameCustomData
    {
      foreach (T data in GetAllDataType<T>(_gameCustomsData))
      {
        if (!_gameSaveSystem.Get().ChosenShopContent.Contains(data.Id) || data.ItemType != itemType)
          continue;

        Debug.Log(data.Name);
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

    private T GetCustomOpponentData<T>(ItemType itemType) where T : GameCustomData
    {
      List<T> list = GetAllDataType<T>(_gameCustomsData).Where(item => item.ItemType == itemType).ToList();
      
      foreach (T data in list.Where(data => list.IndexOf(data) == _opponentSkinIndex))
      {
        // Debug.Log(_opponentSkinIndex);
        // Debug.Log(data.Name);
        return data;
      }

      Debug.LogException(new Exception("Didn't find any chosen data with type " + itemType));
      return null;
    }
  }
}