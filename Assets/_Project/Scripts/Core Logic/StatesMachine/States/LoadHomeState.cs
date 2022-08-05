using System;
using System.Threading.Tasks;
using _Project.Scripts.Features.HomeScreen;
using _Project.Scripts.Logic.StatesMachine.Interfaces;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using AppacheAds.Scripts.Interfaces;
using AppacheAnalytics.Scripts.Interfaces;
using AppacheRemote.Scripts.Interfaces;

namespace _Project.Scripts.Core_Logic.StatesMachine.States
{
  public class LoadHomeState : IState
  {
    private readonly IRemote _remote;
    private readonly IAnalytics _analytics;
    private readonly IAdsService _adsService;
    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;
    private readonly IMoneyService _moneyService;
    private readonly ICurrentScreen _currentScreen;
    private readonly IAssetProvider _assetProvider;
    private readonly IGameSaveSystem _gameSaveSystem;

    private IGameStateMachine _gameStateMachine;

    private HomeScreenLogic _homeScreenLogic;

    public LoadHomeState(IAssetProvider assetProvider, ICurrentScreen currentScreen,
      IGameSaveSystem gameSaveSystem, IAdsService adsService, IAnalytics analytics,
      ILocaleSystem localeSystem, IAudioService audioService, IRemote remote, IMoneyService moneyService)
    {
      _remote = remote;
      _analytics = analytics;
      _adsService = adsService;
      _localeSystem = localeSystem;
      _audioService = audioService;
      _moneyService = moneyService;
      _assetProvider = assetProvider;
      _currentScreen = currentScreen;
      _gameSaveSystem = gameSaveSystem;
    }

    public void Construct(IGameStateMachine gameStateMachine) =>
      _gameStateMachine = gameStateMachine;

    public void Enter()
    {
      _homeScreenLogic = new HomeScreenLogic(_gameStateMachine, _assetProvider, _currentScreen, _gameSaveSystem, _adsService,
        _analytics, _localeSystem, _audioService, _remote, _moneyService);
      _homeScreenLogic.CreateHomeScreen();
    }

    public void Exit() =>
      _homeScreenLogic.CloseHomeScreen();

    public Task AsyncEnter() =>
      throw new NotImplementedException();
  }
}