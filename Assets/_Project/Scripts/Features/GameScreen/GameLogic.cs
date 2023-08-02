using _Project.Scripts.Core_Logic.Screens;
using _Project.Scripts.Core_Logic.StatesMachine.Interfaces;
using _Project.Scripts.Core_Logic.StatesMachine.States;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Features.Loading;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using SableGamesAds.Scripts.Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.GameScreen
{
  public class GameLogic
  {
    private readonly IGameFactory _gameFactory;

    private readonly IAdsService _adsService;
    private readonly ILoadingLogic _loadingLogic;
    private readonly IInputResolver _inputResolver;
    private readonly IGameMetaProvider _gameMetaProvider;
    private readonly IGameStateMachine _gameStateMachine;

    public GameLogic(IGameStateMachine gameStateMachine, IAssetProvider assetProvider, IAdsService adsService, ILocaleSystem localeSystem,
      IAudioService audioService, IGameMetaProvider gameMetaProvider, IInputResolver inputResolver, ILoadingLogic loadingLogic)
    {
      _adsService = adsService;
      _loadingLogic = loadingLogic;
      _inputResolver = inputResolver;
      _gameMetaProvider = gameMetaProvider;
      _gameStateMachine = gameStateMachine;
      
      _gameFactory = new GameFactory(assetProvider, localeSystem, audioService, gameMetaProvider, inputResolver);
    }

    public void Enter()
    {
      _inputResolver.Disable();

      _gameFactory.CreateScreen();
      _gameFactory.CreateField();
      _gameFactory.CreateEndGame();

      _gameFactory.EndGameLogic.Construct(OnClickBack, OnClickPlayAgain);
      _gameFactory.GameFieldLogic.Construct(OnEndGame);
      _gameFactory.GameViewLogic.Construct(OnClickBack, OnEndGame);
      _gameFactory.GameViewLogic.OpenWindow();

      if (_gameMetaProvider.GetEnemyType() == EnemyType.OneDevice)
      {
        _inputResolver.Enable();
        return;
      }
      
      _loadingLogic.Enter(() => _inputResolver.Enable());
    }

    public void Exit()
    {
      _gameFactory.ClearScreen();
    }

    private void OnClickBack()
    {
      _gameMetaProvider.ClearScore();
      
      _adsService.CurrentInterstitial.ShowInterstitial(ScreenNames.Home,
        _gameStateMachine.Enter<LoadHomeState>,
        _gameStateMachine.Enter<LoadHomeState>);
    }
    
    private void OnClickPlayAgain() => 
      _gameStateMachine.Enter<LoadGameState>();

    private void OnEndGame(SymbolSign symbol)
    {
      Debug.Log("Win is " + symbol);
      
      _inputResolver.Disable();
      _gameFactory.EndGameLogic.Show(symbol);
      
      if(_gameMetaProvider.GetGame() != Game.Buhurt)
        return;
      
      _gameFactory.GameViewLogic.HidePlayersPanels();
    }
  }
}