using System;
using System.Collections.Generic;
using _Project.Scripts.Core_Logic.Common;
using _Project.Scripts.Features.Field;
using _Project.Scripts.Features.Field.Mono;
using _Project.Scripts.Features.GameScreen.Mono;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Network;
using UnityEngine;

namespace _Project.Scripts.Features.GameScreen
{
  public class GameFactory : GameObjectFactory, IGameFactory
  {
    private const string CanvasPath = "Canvas";
    
    private const string PlayerPath = "Features/Player/Player";
    private const string GameViewPath = "Features/GameScreen/Game Window";
    private const string GameFieldPath = "Features/GameScreen/Fields/Game Field";
    
    private GameViewMono GameViewMono { get; set; }
    private GameField GameField { get; set; }
    
    public IGameFieldLogic GameFieldLogic { get; private set; }
    public IGameViewLogic GameViewLogic { get; private set; }

    private readonly IAudioService _audioService;
    private readonly IGameMetaProvider _gameMetaProvider;
    
    private readonly Dictionary<ConnectionMode, Action> _dictionary;
    
    
    
    // private delegate void ConnectHandler(string guid, uint cost, Action onSpend, Action<UnlockType> onFail); example delegate

    public GameFactory(IAssetProvider assetProvider, ILocaleSystem localeSystem, IAudioService audioService, INetwork network,
      IGameMetaProvider gameMetaProvider) : base(assetProvider)
    {
      _audioService = audioService;
      _gameMetaProvider = gameMetaProvider;
      
      _dictionary = new Dictionary<ConnectionMode, Action>
      {
        {ConnectionMode.Single, () => GameViewLogic = new SingleGameViewLogic(GameViewMono, localeSystem, network, _gameMetaProvider, _audioService)},
        {ConnectionMode.Network, () => GameViewLogic = new MultiplayerGameViewLogic(GameViewMono, localeSystem, network, _gameMetaProvider, _audioService)},
      };

    }
    
    public void CreateScreen()
    {
      GameViewMono = Create<GameViewMono>(GameViewPath, GameObject.Find(CanvasPath).transform);
      
      _gameMetaProvider.SetConnectionMode(ConnectionMode.Network);
      _dictionary[_gameMetaProvider.GetConnectionMode()].Invoke();
    }
    
    public void CreateField()
    {
      GameField = Create<GameField>(GameFieldPath);
      GameFieldLogic = new GameFieldLogic(GameField, _audioService, _gameMetaProvider);
    }
    
    public void SpawnPlayerObject()
    {
      var playerViewMono = Create<PlayerViewMono>(PlayerPath);
      _gameMetaProvider.SetPlayerViewMono(playerViewMono);
    }

    public void ClearScreen()
    {
      if (GameViewMono != null)
      {
        Clear(GameViewMono.gameObject);
        GameViewMono = null;
      }
      
      if (GameField != null)
      {
        Clear(GameField.gameObject);
        GameField = null;
      }
    }

  }
}