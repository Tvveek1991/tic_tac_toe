using System;
using System.Collections.Generic;
using _Project.Scripts.Core_Logic.Common;
using _Project.Scripts.Features.EndGameScreen;
using _Project.Scripts.Features.EndGameScreen.Mono;
using _Project.Scripts.Features.Field;
using _Project.Scripts.Features.Field.Mono;
using _Project.Scripts.Features.GameScreen.Mono;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
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
    private const string EndGameScreenPath = "Features/GameScreen/End Game Screen";

    private GameViewMono GameViewMono { get; set; }
    private GameField GameField { get; set; }
    private EndGameMono EndGameWindow { get; set; }

    public IGameFieldLogic GameFieldLogic { get; private set; }
    public IGameViewLogic GameViewLogic { get; private set; }
    public IEndGameLogic EndGameLogic { get; private set; }

    private readonly IAudioService _audioService;
    private readonly ILocaleSystem _localeSystem;
    private readonly IGameMetaProvider _gameMetaProvider;

    private readonly Dictionary<EnemyType, Action> _dictionaryGameMode;

    // private delegate void ConnectHandler(string guid, uint cost, Action onSpend, Action<UnlockType> onFail); example delegate

    public GameFactory(IAssetProvider assetProvider, ILocaleSystem localeSystem, IAudioService audioService, 
      IGameMetaProvider gameMetaProvider, IInputResolver inputResolver) : base(assetProvider)
    {
      _audioService = audioService;
      _localeSystem = localeSystem;
      _gameMetaProvider = gameMetaProvider;

      _dictionaryGameMode = new Dictionary<EnemyType, Action>
      {
        {EnemyType.Bot, () => GameViewLogic = new SingleGameViewLogic(GameViewMono, this, localeSystem, _gameMetaProvider, _audioService, inputResolver)},
        {EnemyType.OneDevice, () => GameViewLogic = new SingleGameViewLogic(GameViewMono, this, localeSystem, _gameMetaProvider, _audioService, inputResolver)},
        // {GameMode.Multiplayer, () => GameViewLogic = new MultiplayerGameViewLogic(GameViewMono, localeSystem, network, _gameMetaProvider, _audioService, inputResolver)},
      };
    }

    public void CreateScreen()
    {
      GameViewMono = Create<GameViewMono>(GameViewPath, GameObject.Find(CanvasPath).transform);
      _dictionaryGameMode[_gameMetaProvider.GetEnemyType()].Invoke();
    }

    public void CreateField()
    {
      GameField = Create<GameField>(GameFieldPath);
      GameFieldLogic = new GameFieldLogic(GameField, _audioService, _gameMetaProvider);
    }
    public void CreateEndGame()
    {
      EndGameWindow = Create<EndGameMono>(EndGameScreenPath, GameObject.Find(CanvasPath).transform);
      EndGameLogic = new EndGameLogic(EndGameWindow, _audioService, _localeSystem, _gameMetaProvider);
    }

    public void SpawnPlayerObject()
    {
      var playerViewMono = Create<BasePlayerViewMono>(_gameMetaProvider.GetGame() == Game.Classic ? ClassicPlayerPath : BuhurtPlayerPath);
      
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
      
      if (EndGameWindow != null)
      {
        Clear(EndGameWindow.gameObject);
        EndGameWindow = null;
      }
    }
  }
}