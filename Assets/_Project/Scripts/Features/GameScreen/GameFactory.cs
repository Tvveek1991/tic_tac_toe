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
using UnityEngine.Animations;

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
    private const string WarningViewPath = "Features/GameScreen/Warning View";
    
    private readonly IAudioService _audioService;
    private readonly ILocaleSystem _localeSystem;
    private readonly IInputResolver _inputResolver;
    private readonly IGameMetaProvider _gameMetaProvider;

    private readonly Dictionary<EnemyType, Action> _dictionaryGameMode;

    private readonly Transform _canvasParent;

    public WarningView WarningView { get; private set; }
    public BasePlayerViewMono PlayerViewMono { get; private set; }
    public IGameFieldLogic GameFieldLogic { get; private set; }
    public IGameViewLogic GameViewLogic { get; private set; }
    public IEndGameLogic EndGameLogic { get; private set; }
    
    private GameViewMono GameViewMono { get; set; }
    private GameField GameField { get; set; }
    private EndGameMono EndGameWindow { get; set; }


    // private delegate void ConnectHandler(string guid, uint cost, Action onSpend, Action<UnlockType> onFail); example delegate

    public GameFactory(IAssetProvider assetProvider, ILocaleSystem localeSystem, IAudioService audioService,
      IGameMetaProvider gameMetaProvider, IInputResolver inputResolver, IGameSaveSystem gameSaveSystem) : base(assetProvider)
    {
      _audioService = audioService;
      _inputResolver = inputResolver;
      _localeSystem = localeSystem;
      _gameMetaProvider = gameMetaProvider;

      _dictionaryGameMode = new Dictionary<EnemyType, Action>
      {
        {EnemyType.Bot, () => GameViewLogic = new BotGameViewLogic(GameViewMono, this, localeSystem, 
          _gameMetaProvider, _audioService, inputResolver, assetProvider, gameSaveSystem)},
        {EnemyType.OneDevice, () => GameViewLogic = new OneDeviceGameViewLogic(GameViewMono, this, localeSystem, 
          _gameMetaProvider, _audioService, inputResolver, assetProvider, gameSaveSystem)},
      };
      
      _canvasParent = GameObject.Find(CanvasPath).transform;
    }

    public void CreateScreen()
    {
      GameViewMono = Create<GameViewMono>(GameViewPath, _canvasParent);
      _dictionaryGameMode[_gameMetaProvider.GetEnemyType()].Invoke();
    }

    public void CreateField()
    {
      GameField = Create<GameField>(GameFieldPath, _canvasParent);
      GameFieldLogic = new GameFieldLogic(GameField, _audioService, _gameMetaProvider);
    }

    public void CreateEndGame()
    {
      EndGameWindow = Create<EndGameMono>(EndGameScreenPath, _canvasParent);
      EndGameLogic = new EndGameLogic(EndGameWindow, _audioService, _localeSystem, _gameMetaProvider);
    }

    public void CreateWarningView() => 
      WarningView = Create<WarningView>(WarningViewPath, _canvasParent);

    public void SpawnPlayerObject()
    {
      PlayerViewMono = Create<BasePlayerViewMono>(_gameMetaProvider.GetGame() == Game.Classic ? ClassicPlayerPath : BuhurtPlayerPath);
      PlayerViewMono.Construct(this, _gameMetaProvider, _inputResolver);
      _gameMetaProvider.SetPlayerViewMono(PlayerViewMono);
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

      if (PlayerViewMono != null)
      {
        Clear(PlayerViewMono.gameObject);
        PlayerViewMono = null;
      }
      
      if (WarningView != null)
      {
        Clear(WarningView.gameObject);
        WarningView = null;
      }
    }
  }
}