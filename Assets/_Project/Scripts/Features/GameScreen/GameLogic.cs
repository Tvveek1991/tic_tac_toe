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
      IAudioService audioService, IGameMetaProvider gameMetaProvider, IInputResolver inputResolver, IGameSaveSystem gameSaveSystem, ILoadingLogic loadingLogic)
    {
      _adsService = adsService;
      _loadingLogic = loadingLogic;
      _inputResolver = inputResolver;
      _gameMetaProvider = gameMetaProvider;
      _gameStateMachine = gameStateMachine;
      
      _gameFactory = new GameFactory(assetProvider, localeSystem, audioService, gameMetaProvider, inputResolver, gameSaveSystem);
    }

    public void Enter()
    {
      _inputResolver.Disable();
      
      LoadGame();
      _gameFactory.GameViewLogic.StartGame();
    }

    public void Exit()
    {
      _gameFactory.ClearScreen();
      _gameFactory.GameViewLogic.Clear();
    }

    private void LoadGame()
    {
      _gameFactory.CreateScreen();
      _gameFactory.CreateField();
      _gameFactory.CreateEndGame();

      _gameFactory.EndGameLogic.Construct(OnClickBack, OnClickPlayAgain);
      _gameFactory.GameFieldLogic.Construct(OnEndGame);
      _gameFactory.GameViewLogic.Construct(OnClickBack, OnEndGame);
    }

    //Move back to homeScreen
    private void OnClickBack()
    {
      _loadingLogic.Enter(() =>
      {
        _gameMetaProvider.ClearScore();
        _adsService.CurrentInterstitial.ShowInterstitial(ScreenNames.Home,
          _gameStateMachine.Enter<LoadHomeState>,
          _gameStateMachine.Enter<LoadHomeState>);
      });
      
    }
    
    //Remove to gameScreen
    private void OnClickPlayAgain()
    {
      _adsService.CurrentInterstitial.ShowInterstitial(ScreenNames.Game,
        _gameStateMachine.Enter<LoadGameState>,
        _gameStateMachine.Enter<LoadGameState>);
    }

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