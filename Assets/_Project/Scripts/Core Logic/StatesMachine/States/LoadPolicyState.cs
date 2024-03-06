using System.Threading.Tasks;
using _Project.Scripts.Core_Logic.Screens;
using _Project.Scripts.Core_Logic.StatesMachine.Interfaces;
using _Project.Scripts.Features.Policy;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using SableGamesAnalytics.Scripts.Interfaces;
using SableGamesRemote.Scripts.Interfaces;

namespace _Project.Scripts.Core_Logic.StatesMachine.States
{
  public class LoadPolicyState : IState
  {
    private IGameStateMachine _gameStateMachine;
    
    private readonly IAssetProvider _assetProvider;
    private readonly IGameSaveSystem _gameSaveSystem;
    private readonly ICurrentScreen _screenManager;
    
    private IPrivacyPolicyFactory _privacyPolicyFactory;

    public LoadPolicyState(
      IAssetProvider assetProvider,
      IGameSaveSystem gameSaveSystem,
      ICurrentScreen screenManager)
    {
      _assetProvider = assetProvider;
      _gameSaveSystem = gameSaveSystem;
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
        _gameSaveSystem.Get().IsPolicyAccepted = true;
        _gameSaveSystem.Save();

        Next();
      }
      /*if (!_gameSaveSystem.Get().IsPolicyAccepted)
      {
        var privacyPolicy = _privacyPolicyFactory.SpawnPrivacyPolicy();
        privacyPolicy.Construct(_gameSaveSystem, _audioService, _localeSystem, _analytics, _remote);
        privacyPolicy.ClosingFinished += OnClosingFinished;
        privacyPolicy.OpenWindow();

        void OnClosingFinished()
        {
          Next();
          privacyPolicy.ClosingFinished -= OnClosingFinished;
        }
      }*/
      else
        Next();
    }

    private void Next() => 
      _gameStateMachine.Enter<LoadHomeState>();


    public void Exit() =>
      _privacyPolicyFactory.Clear();
  }
}