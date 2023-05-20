using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Features.ShopScreen.Data;
using _Project.Scripts.Features.ShopScreen.Serialized;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Unlocker;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Services.GameMetaProvider
{
  public class GameMetaProvider : IGameMetaProvider
  {
    private const string SymbolsDataPath = "Data/Game/Symbols";
    
    private readonly IAssetProvider _assetProvider;
    private readonly IUnlockService _unlockService;
    private readonly IGameSaveSystem _gameSaveSystem;

    private readonly BaseContentData[] _itemsData;

    private BasePlayerViewMono BasePlayerViewMono { get; set; }


    private Dictionary<SymbolSign, PlayerData> _playerData;

    public GameMetaProvider(IAssetProvider assetProvider, IGameSaveSystem gameSaveSystem, IUnlockService unlockService)
    {
      _assetProvider = assetProvider;
      _unlockService = unlockService;
      _gameSaveSystem = gameSaveSystem;

      _itemsData = Load<BaseContentData>(SymbolsDataPath);

      if (_gameSaveSystem.Get().IsFirstSession)
        OpenFree();

      CreatePlayerData();
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

    public Mesh GetMesh(DragableData dragableData) =>
      GetGame() == Game.Classic
        ? GetPlayerData()[dragableData.GetSymbol()].SymbolData.MeshClassic
        : GetPlayerData()[dragableData.GetSymbol()].SymbolData.MeshBuhurt[(int) dragableData.GetSize()];

    public Material GetMaterial(DragableData dragableData) =>
      GetGame() == Game.Classic
        ? GetPlayerData()[dragableData.GetSymbol()].SymbolData.MaterialClassic
        : GetPlayerData()[dragableData.GetSymbol()].SymbolData.MaterialBuhurt[(int) dragableData.GetSize()];

    public void Clear()
    {
      if (BasePlayerViewMono)
        Object.Destroy(BasePlayerViewMono.gameObject);
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
            SymbolData = GetSymbolData<SymbolData>(ItemType.SymbolX)
          }
        },
        {
          SymbolSign.O, new PlayerData()
          {
            Sign = SymbolSign.O,
            Color = Color.blue,
            SymbolData = GetSymbolData<SymbolData>(ItemType.SymbolO)
          }
        }
      };
    }

    private void OpenFree()
    {
      foreach (BaseContentData symbolData in _itemsData)
      {
        if (IsUnlocked(symbolData.Id) || _unlockService.GetUnlockType(symbolData.Id) != UnlockType.Free)
          continue;

        _gameSaveSystem.Get().UnlockedShopContent.Add(symbolData.Id);
        _gameSaveSystem.Get().ChosenShopContent.Add(symbolData.Id);
      }

      _gameSaveSystem.Get().IsFirstSession = false;
      _gameSaveSystem.Save();
    }

    private T[] GetAllTypeData<T>(BaseContentData[] baseContentsData) where T : BaseContentData
    {
      var filteredData = new List<T>();

      foreach (BaseContentData data in baseContentsData)
        if (data is T symbolData)
          filteredData.Add(symbolData);

      return filteredData.ToArray();
    }
    
    private SymbolData GetSymbolData<T>(ItemType symbolSign)
    {
      foreach (SymbolData symbolData in GetAllTypeData<SymbolData>(_itemsData))
      {
        if (!_gameSaveSystem.Get().ChosenShopContent.Contains(symbolData.Id) || symbolData.ItemType != symbolSign)
          continue;
        
        return symbolData;
      }

      Debug.LogException(new Exception("Didn't find any chosen symbolData with sign " + symbolSign));
      return null;
    }

    private bool IsUnlocked(string itemId) =>
      _gameSaveSystem.Get().UnlockedShopContent.Contains(itemId);

    private T[] Load<T>(string path) where T : BaseContentData =>
      _assetProvider.GetAllResources<T>(path);
  }

  [Serializable]
  public class PlayerData
  {
    public SymbolSign Sign;
    public Color Color;
    public SymbolData SymbolData;
  }
}