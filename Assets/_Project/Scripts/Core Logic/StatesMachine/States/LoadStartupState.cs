using System.Threading.Tasks;
using _Project.Scripts.Core_Logic.StatesMachine.Interfaces;
using _Project.Scripts.Services._Interfaces;

namespace _Project.Scripts.Core_Logic.StatesMachine.States
{
  public class LoadStartupState : IState
  {
    private readonly IAudioService _audioService;
    private readonly IGameSaveSystem _gameSaveSystem;
    
    private IGameStateMachine _gameStateMachine;

    public LoadStartupState(IAudioService audioService, IGameSaveSystem gameSaveSystem)
    {
      _audioService = audioService;
      _gameSaveSystem = gameSaveSystem;
    }

    public void Construct(IGameStateMachine gameStateMachine) => 
      _gameStateMachine = gameStateMachine;

    public void Exit() { }
    
    public Task AsyncEnter() => 
      throw new System.NotImplementedException();
    
    public void Enter()
    {
      LoadSound();
      _gameStateMachine.Enter<LoadPolicyState>();
    }
    

    private void LoadSound()
    {
      _audioService.MuteMusic(_gameSaveSystem.Get().IsMusicDisabled);
      _audioService.MuteSounds(_gameSaveSystem.Get().IsSoundDisabled);
    }
  }
}