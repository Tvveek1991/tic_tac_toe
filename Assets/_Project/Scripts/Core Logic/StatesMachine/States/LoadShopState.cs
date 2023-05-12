using System;
using System.Threading.Tasks;
using _Project.Scripts.Core_Logic.StatesMachine.Interfaces;
using _Project.Scripts.Features.ShopScreen;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using AppacheAds.Scripts.Interfaces;
using AppacheAnalytics.Scripts.Interfaces;
using AppacheRemote.Scripts.Interfaces;

namespace _Project.Scripts.Core_Logic.StatesMachine.States
{
  public class LoadShopState : IState
  {
    private readonly IRemote _remote;
    private readonly IAnalytics _analytics;
    private readonly IAdsService _adsService;
    private readonly IMoneyService _moneyService;
    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;
    private readonly IUnlockService _unlockService;
    private readonly ICurrentScreen _currentScreen;
    private readonly IAssetProvider _assetProvider;
    private readonly IGameSaveSystem _gameSaveSystem;
    private readonly IGameMetaProvider _gameMetaProvider;

    private IGameStateMachine _gameStateMachine;

    private ShopLogic _shopLogic;

    private LoadShopState(IAssetProvider assetProvider, ICurrentScreen currentScreen, IGameSaveSystem gameSaveSystem,
      IAdsService adsService, IAnalytics analytics, ILocaleSystem localeSystem, IAudioService audioService, IRemote remote,
      IUnlockService unlockService, IMoneyService moneyService, IGameMetaProvider gameMetaProvider)
    {
      _remote = remote;
      _analytics = analytics;
      _adsService = adsService;
      _moneyService = moneyService;
      _localeSystem = localeSystem;
      _audioService = audioService;
      _unlockService = unlockService;
      _assetProvider = assetProvider;
      _currentScreen = currentScreen;
      _gameSaveSystem = gameSaveSystem;
      _gameMetaProvider = gameMetaProvider;
    }

    public void Construct(IGameStateMachine gameStateMachine) =>
      _gameStateMachine = gameStateMachine;

    public void Enter()
    {
      _shopLogic = new ShopLogic(_gameStateMachine, _assetProvider, _currentScreen, _gameSaveSystem, _adsService,
        _analytics, _localeSystem, _audioService, _unlockService, _gameMetaProvider);
      _shopLogic.CreateShopView();
    }

    public void Exit() =>
      _shopLogic.ClearShopView();

    public Task AsyncEnter() =>
      throw new NotImplementedException();
  }
}