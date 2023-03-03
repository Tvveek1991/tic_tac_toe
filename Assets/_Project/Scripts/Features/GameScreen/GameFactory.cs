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

    private const string ClassicPlayerPath = "Features/Player/Classic Player";
    private const string BuhurtPlayerPath = "Features/Player/Buhurt Player";
    
    private const string GameViewPath = "Features/GameScreen/Game Window";
    private const string GameFieldPath = "Features/GameScreen/Fields/Game Field";

    private GameViewMono GameViewMono { get; set; }
    private GameField GameField { get; set; }

    public IGameFieldLogic GameFieldLogic { get; private set; }
    public IGameViewLogic GameViewLogic { get; private set; }

    private readonly IAudioService _audioService;
    private readonly IGameMetaProvider _gameMetaProvider;

    private readonly Dictionary<GameMode, Action> _dictionaryGameMode;

    // private delegate void ConnectHandler(string guid, uint cost, Action onSpend, Action<UnlockType> onFail); example delegate

    public GameFactory(IAssetProvider assetProvider, ILocaleSystem localeSystem, IAudioService audioService, INetwork network,
      IGameMetaProvider gameMetaProvider) : base(assetProvider)
    {
      _audioService = audioService;
      _gameMetaProvider = gameMetaProvider;

      _dictionaryGameMode = new Dictionary<GameMode, Action>
      {
        {GameMode.Single, () => GameViewLogic = new SingleGameViewLogic(GameViewMono, this, localeSystem, _gameMetaProvider, _audioService)},
        {GameMode.Multiplayer, () => GameViewLogic = new MultiplayerGameViewLogic(GameViewMono, localeSystem, network, _gameMetaProvider, _audioService)},
      };
    }

    public void CreateScreen()
    {
      GameViewMono = Create<GameViewMono>(GameViewPath, GameObject.Find(CanvasPath).transform);
      _dictionaryGameMode[_gameMetaProvider.GetGameMode()].Invoke();
    }

    public void CreateField()
    {
      GameField = Create<GameField>(GameFieldPath);
      GameFieldLogic = new GameFieldLogic(GameField, _audioService, _gameMetaProvider);
    }

    public void SpawnPlayerObject()
    {
      BasePlayerViewMono playerViewMono = (_gameMetaProvider.GetGame() == Game.Classic ? (BasePlayerViewMono) Create<ClassicPlayerViewMono>(ClassicPlayerPath) : Create<BuhurtPlayerViewMono>(BuhurtPlayerPath));
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