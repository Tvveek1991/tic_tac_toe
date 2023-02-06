using _Project.Scripts.Core_Logic.StatesMachine.Interfaces;
using _Project.Scripts.Core_Logic.StatesMachine.States;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Network;
using AppacheAds.Scripts.Interfaces;
using AppacheAnalytics.Scripts.Interfaces;
using AppacheRemote.Scripts.Interfaces;

namespace _Project.Scripts.Features.GameScreen
{
  public class GameLogic
  {
    private readonly IUiFactory _uiFactory;
    private readonly IGameFactory _gameFactory;

    private readonly IRemote _remote;
    private readonly INetwork _network;
    private readonly IAnalytics _analytics;
    private readonly IAdsService _adsService;
    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;
    private readonly ICurrentScreen _currentScreen;
    private readonly IGameSaveSystem _gameSaveSystem;
    private readonly IGameMetaProvider _gameMetaProvider;
    private readonly IGameStateMachine _gameStateMachine;
    private readonly IAssetProvider _assetProvider;
    
    public GameLogic(IGameStateMachine gameStateMachine, IAssetProvider assetProvider, ICurrentScreen currentScreen,
      IGameSaveSystem gameSaveSystem, IAdsService adsService, IAnalytics analytics, ILocaleSystem localeSystem,
      IAudioService audioService, IRemote remote, INetwork network, IGameMetaProvider gameMetaProvider)
    {
      _remote = remote;
      _network = network;
      _analytics = analytics;
      _adsService = adsService;
      _localeSystem = localeSystem;
      _audioService = audioService;
      _currentScreen = currentScreen;
      _assetProvider = assetProvider;
      _gameSaveSystem = gameSaveSystem;
      _gameMetaProvider = gameMetaProvider;
      _gameStateMachine = gameStateMachine;

      _uiFactory = new UiFactory(assetProvider);
      _gameFactory = new GameFactory(assetProvider, localeSystem, audioService, network, gameMetaProvider);
    }

    public void Enter(bool isHost)
    {
      _gameFactory.CreateField();
      _gameFactory.BaseFieldLogic.Construct(_gameMetaProvider);
      
      _gameFactory.CreateScreen();
      _gameFactory.GameViewLogic.Construct(_audioService, _gameFactory, OnClickBack, OnLoadClient);
      _gameFactory.GameViewLogic.OpenWindow();

      ConnectClient(isHost);
    }

    public void Exit() => 
      _gameFactory.ClearScreen();

    private void OnClickBack()
    {
      _gameStateMachine.Enter<LoadHomeState>();
      /*_adsService.CurrentInterstitial.ShowInterstitial(ScreenNames.Home,
        _gameStateMachine.Enter<LoadHomeState>,
        _gameStateMachine.Enter<LoadHomeState>);*/
    }

    private void ConnectClient(bool isHost) =>
      _network.ConnectClient(isHost);

    private void OnLoadClient() => 
      _gameFactory.SpawnPlayerObject();

    private void OnChangeTurn()
    {
      
    }
  }
}