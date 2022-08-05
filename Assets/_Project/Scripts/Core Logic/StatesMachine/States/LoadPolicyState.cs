using System.Threading.Tasks;
using _Project.Scripts.Core_Logic.Screens;
using _Project.Scripts.Features.Policy;
using _Project.Scripts.Logic.StatesMachine.Interfaces;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using AppacheAnalytics.Scripts.Interfaces;
using UnityEngine;

namespace _Project.Scripts.Core_Logic.StatesMachine.States
{
  public class LoadPolicyState : IState
  {
    private IGameStateMachine _gameStateMachine;
    
    private readonly IAssetProvider _assetProvider;
    private readonly IGameSaveSystem _gameSaveSystem;
    private readonly IAudioService _audioService;
    private readonly ILocaleSystem _localeSystem;
    private readonly ICurrentScreen _screenManager;
    private readonly IAnalytics _analytics;
    
    private IPrivacyPolicyFactory _privacyPolicyFactory;

    public LoadPolicyState(
      IAssetProvider assetProvider,
      IGameSaveSystem gameSaveSystem,
      IAudioService audioService,
      ILocaleSystem localeSystem,
      ICurrentScreen screenManager,
      IAnalytics analytics)
    {
      _analytics = analytics;
      _assetProvider = assetProvider;
      _gameSaveSystem = gameSaveSystem;
      _audioService = audioService;
      _localeSystem = localeSystem;
      _screenManager = screenManager;
    }

    public void Construct(IGameStateMachine gameStateMachine) => 
      _gameStateMachine = gameStateMachine;

    public Task AsyncEnter() =>
      throw new System.NotImplementedException();

    public void Enter()
    {
      _screenManager.SetCurrentScreen(ScreenNames.Privacy);

      _privacyPolicyFactory = new PrivacyPolicyFactory(_assetProvider);
      
      if (!_gameSaveSystem.Get().IsPolicyAccepted)
      {
        var privacyPolicy = _privacyPolicyFactory.SpawnPrivacyPolicy();
        privacyPolicy.Construct(_gameSaveSystem, _audioService, _localeSystem, _analytics);
        privacyPolicy.ClosingFinished += OnClosingFinished;
        privacyPolicy.OpenWindow();

        void OnClosingFinished()
        {
          Next();
          privacyPolicy.ClosingFinished -= OnClosingFinished;
        }
      }
      else
        Next();

      LoadSound();
    }

    private void LoadSound()
    {
      _audioService.MuteMusic(_gameSaveSystem.Get().IsMusicDisabled);
      _audioService.MuteSounds(_gameSaveSystem.Get().IsSoundDisabled);
    }

    private void Next() => 
      _gameStateMachine.Enter<LoadHomeState>();


    public void Exit() =>
      _privacyPolicyFactory.Clear();
  }
}