using _Project.Scripts.Core_Logic.Screens;
using _Project.Scripts.Core_Logic.StatesMachine.Interfaces;
using _Project.Scripts.Core_Logic.StatesMachine.States;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Network;
using AppacheAds.Scripts.Interfaces;
using AppacheAnalytics.Scripts.Interfaces;
using AppacheRemote.Scripts.Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.GameScreen
{
  public class GameLogic
  {
    private readonly IUiFactory _uiFactory;
    private readonly IGameFactory _gameFactory;

    private readonly IRemote _remote;
    private readonly IAnalytics _analytics;
    private readonly IAdsService _adsService;
    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;
    private readonly IInputResolver _inputResolver;
    private readonly ICurrentScreen _currentScreen;
    private readonly IGameSaveSystem _gameSaveSystem;
    private readonly IGameMetaProvider _gameMetaProvider;
    private readonly IGameStateMachine _gameStateMachine;
    private readonly IAssetProvider _assetProvider;

    public GameLogic(IGameStateMachine gameStateMachine, IAssetProvider assetProvider, ICurrentScreen currentScreen,
      IGameSaveSystem gameSaveSystem, IAdsService adsService, IAnalytics analytics, ILocaleSystem localeSystem,
      IAudioService audioService, IRemote remote, IGameMetaProvider gameMetaProvider, IInputResolver inputResolver)
    {
      _remote = remote;
      _analytics = analytics;
      _adsService = adsService;
      _localeSystem = localeSystem;
      _audioService = audioService;
      _inputResolver = inputResolver;
      _currentScreen = currentScreen;
      _assetProvider = assetProvider;
      _gameSaveSystem = gameSaveSystem;
      _gameMetaProvider = gameMetaProvider;
      _gameStateMachine = gameStateMachine;

      _uiFactory = new UiFactory(assetProvider);
      _gameFactory = new GameFactory(assetProvider, localeSystem, audioService, gameMetaProvider, inputResolver);
    }

    public void Enter(bool isHost)
    {
      _inputResolver.Enable();
      
      // _network.CreateClientDataTransmitter(_gameFactory, _gameMetaProvider, _inputResolver);
      
      _gameFactory.CreateField();
      _gameFactory.CreateScreen();
      _gameFactory.CreateEndGame();

      _gameFactory.EndGameLogic.Construct(OnClickBack, OnClickPlayAgain);
      _gameFactory.GameFieldLogic.Construct(OnWin);
      _gameFactory.GameViewLogic.Construct(OnClickBack, OnCreateNonConnectionPlayer);
      _gameFactory.GameViewLogic.OpenWindow();
    }

    public void Exit()
    {
      _gameFactory.ClearScreen();
      _gameMetaProvider.Clear();
    }

    private void OnClickBack()
    {
      _adsService.CurrentInterstitial.ShowInterstitial(ScreenNames.Home,
        _gameStateMachine.Enter<LoadHomeState>,
        _gameStateMachine.Enter<LoadHomeState>);
    }
    
    private void OnClickPlayAgain() => 
      _gameStateMachine.Enter<LoadGameState, bool>(false);


    private void OnCreateNonConnectionPlayer() =>
      _gameFactory.SpawnPlayerObject();

    private void OnWin(SymbolSign symbol)
    {
      _inputResolver.Disable();
      _gameFactory.EndGameLogic.Show(symbol);
      Debug.Log("Win is " + symbol);
    }
  }
}