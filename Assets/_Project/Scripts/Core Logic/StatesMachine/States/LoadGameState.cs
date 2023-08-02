using System;
using System.Threading.Tasks;
using _Project.Scripts.Core_Logic.StatesMachine.Interfaces;
using _Project.Scripts.Features.GameScreen;
using _Project.Scripts.Features.Loading;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using SableGamesAds.Scripts.Interfaces;
using SableGamesAnalytics.Scripts.Interfaces;
using SableGamesRemote.Scripts.Interfaces;

namespace _Project.Scripts.Core_Logic.StatesMachine.States
{
  public class LoadGameState : IState
  {
    private readonly IRemote _remote;
    private readonly IAnalytics _analytics;
    private readonly IAdsService _adsService;
    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;
    private readonly IMoneyService _moneyService;
    private readonly ILoadingLogic _loadingLogic;
    private readonly IInputResolver _inputResolver;
    private readonly ICurrentScreen _currentScreen;
    private readonly IAssetProvider _assetProvider;
    private readonly IGameSaveSystem _gameSaveSystem;
    private readonly IGameMetaProvider _gameMetaProvider;

    private IGameStateMachine _gameStateMachine;

    private GameLogic _gameLogic;

    public LoadGameState(IAssetProvider assetProvider, ICurrentScreen currentScreen, IGameSaveSystem gameSaveSystem,
      IAdsService adsService, IAnalytics analytics, ILocaleSystem localeSystem, IAudioService audioService, IRemote remote,
      IMoneyService moneyService, IGameMetaProvider gameMetaProvider, IInputResolver inputResolver, ILoadingLogic loadingLogic)
    {
      _remote = remote;
      _analytics = analytics;
      _adsService = adsService;
      _localeSystem = localeSystem;
      _loadingLogic = loadingLogic;
      _audioService = audioService;
      _moneyService = moneyService;
      _inputResolver = inputResolver;
      _assetProvider = assetProvider;
      _currentScreen = currentScreen;
      _gameSaveSystem = gameSaveSystem;
      _gameMetaProvider = gameMetaProvider;
    }

    public void Construct(IGameStateMachine gameStateMachine) =>
      _gameStateMachine = gameStateMachine;

    public void Enter()
    {
      _gameLogic = new GameLogic(_gameStateMachine, _assetProvider, _adsService,
        _localeSystem, _audioService, _gameMetaProvider, _inputResolver, _loadingLogic);
      _gameLogic.Enter();
    }

    public void Exit() =>
      _gameLogic.Exit();

    public Task AsyncEnter() =>
      throw new NotImplementedException();
  }
}