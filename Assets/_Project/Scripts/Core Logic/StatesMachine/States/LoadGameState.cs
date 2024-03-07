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
    private readonly IAdsService _adsService;
    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;
    private readonly IInputResolver _inputResolver;
    private readonly ILoadingLogic _loadingLogic;
    private readonly IAssetProvider _assetProvider;
    private readonly IGameSaveSystem _gameSaveSystem;
    private readonly IGameMetaProvider _gameMetaProvider;

    private IGameStateMachine _gameStateMachine;

    private GameLogic _gameLogic;

    public LoadGameState(IAssetProvider assetProvider, IGameSaveSystem gameSaveSystem, IAdsService adsService,
      ILocaleSystem localeSystem, IAudioService audioService, IGameMetaProvider gameMetaProvider, IInputResolver inputResolver, 
      ILoadingLogic loadingLogic)
    {
      _adsService = adsService;
      _localeSystem = localeSystem;
      _audioService = audioService;
      _loadingLogic = loadingLogic;
      _inputResolver = inputResolver;
      _assetProvider = assetProvider;
      _gameSaveSystem = gameSaveSystem;
      _gameMetaProvider = gameMetaProvider;
    }

    public void Construct(IGameStateMachine gameStateMachine) =>
      _gameStateMachine = gameStateMachine;

    public void Enter()
    {
      _gameLogic = new GameLogic(_gameStateMachine, _assetProvider, _adsService,
        _localeSystem, _audioService, _gameMetaProvider, _inputResolver, _gameSaveSystem, _loadingLogic);
      _gameLogic.Enter();
    }

    public void Exit() =>
      _gameLogic.Exit();

    public Task AsyncEnter() =>
      throw new NotImplementedException();
  }
}