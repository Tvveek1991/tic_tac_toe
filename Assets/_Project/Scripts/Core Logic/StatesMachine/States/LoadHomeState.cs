using System;
using System.Threading.Tasks;
using _Project.Scripts.Core_Logic.StatesMachine.Interfaces;
using _Project.Scripts.Features.HomeScreen;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Network;
using AppacheAds.Scripts.Interfaces;
using AppacheAnalytics.Scripts.Interfaces;
using AppacheRemote.Scripts.Interfaces;

namespace _Project.Scripts.Core_Logic.StatesMachine.States
{
  public class LoadHomeState : IState
  {
    private readonly IRemote _remote;
    private readonly INetwork _network;
    private readonly IAnalytics _analytics;
    private readonly IAdsService _adsService;
    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;
    private readonly IMoneyService _moneyService;
    private readonly IInAppService _inAppService;
    private readonly ICurrentScreen _currentScreen;
    private readonly IAssetProvider _assetProvider;
    private readonly IGameSaveSystem _gameSaveSystem;
    private readonly IGameMetaProvider _gameMetaProvider;

    private IGameStateMachine _gameStateMachine;

    private HomeLogic _homeLogic;

    public LoadHomeState(IAssetProvider assetProvider, ICurrentScreen currentScreen, IGameSaveSystem gameSaveSystem, IAdsService adsService,
      IAnalytics analytics, ILocaleSystem localeSystem, IAudioService audioService, IRemote remote, IMoneyService moneyService, INetwork network,
      IGameMetaProvider gameMetaProvider, IInAppService inAppService)
    {
      _remote = remote;
      _network = network;
      _analytics = analytics;
      _adsService = adsService;
      _localeSystem = localeSystem;
      _inAppService = inAppService;
      _audioService = audioService;
      _moneyService = moneyService;
      _assetProvider = assetProvider;
      _currentScreen = currentScreen;
      _gameSaveSystem = gameSaveSystem;
      _gameMetaProvider = gameMetaProvider;
    }

    public void Construct(IGameStateMachine gameStateMachine) =>
      _gameStateMachine = gameStateMachine;

    public void Enter()
    {
      _homeLogic = new HomeLogic(_gameStateMachine, _assetProvider, _currentScreen, _gameSaveSystem, _adsService,
        _analytics, _localeSystem, _audioService, _remote, _moneyService, _network, _gameMetaProvider, _inAppService);
      _homeLogic.CreateHomeView();
    }

    public void Exit() =>
      _homeLogic.CloseHomeView();

    public Task AsyncEnter() =>
      throw new NotImplementedException();
  }
}