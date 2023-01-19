using _Project.Scripts.Core_Logic.StatesMachine.States;
using _Project.Scripts.Features.GameScreen.Mono;
using _Project.Scripts.Logic.StatesMachine.Interfaces;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using AppacheAds.Scripts.Interfaces;
using AppacheAnalytics.Scripts.Interfaces;
using AppacheRemote.Scripts.Interfaces;
using Unity.Netcode;

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
    private readonly ICurrentScreen _currentScreen;
    private readonly IGameSaveSystem _gameSaveSystem;
    private readonly IGameStateMachine _gameStateMachine;

    private GameView _gameView;
    private NetworkManager _networkManager;

    public GameLogic(IGameStateMachine gameStateMachine, IAssetProvider assetProvider, ICurrentScreen currentScreen,
      IGameSaveSystem gameSaveSystem, IAdsService adsService, IAnalytics analytics, ILocaleSystem localeSystem,
      IAudioService audioService, IRemote remote)
    {
      _remote = remote;
      _analytics = analytics;
      _adsService = adsService;
      _localeSystem = localeSystem;
      _audioService = audioService;
      _currentScreen = currentScreen;
      _gameSaveSystem = gameSaveSystem;
      _gameStateMachine = gameStateMachine;

      _networkManager = NetworkManager.Singleton;
      
      _uiFactory = new UiFactory(assetProvider);
      _gameFactory = new GameFactory(assetProvider, localeSystem, audioService);
    }

    public void Enter()
    {
      _gameFactory.CreateField();
      _gameFactory.BaseFieldLogic.Construct();
      
      _gameFactory.CreateScreen();
      _gameFactory.GameViewLogic.Construct(_audioService, OnClickBack);
      _gameFactory.GameViewLogic.OpenWindow();
      
    }

    public void Exit()
    {
      _gameFactory.ClearScreen();
    }

    private void OnClickBack()
    {
      _gameStateMachine.Enter<LoadHomeState>();
      /*_adsService.CurrentInterstitial.ShowInterstitial(ScreenNames.Home,
        _gameStateMachine.Enter<LoadHomeState>,
        _gameStateMachine.Enter<LoadHomeState>);*/
    }
    
    private void OnStartHost() =>
      _networkManager.StartHost();
    
    private void OnStartClient() =>
      _networkManager.StartClient();
  }
}