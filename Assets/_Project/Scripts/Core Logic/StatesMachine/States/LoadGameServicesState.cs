using System.Threading.Tasks;
using _Project.Scripts.Core_Logic.StatesMachine.Interfaces;

namespace _Project.Scripts.Core_Logic.StatesMachine.States
{
  public class LoadGameServicesState : IState
  {
    private IGameStateMachine _gameStateMachine;

    public void Construct(IGameStateMachine gameStateMachine) => 
      _gameStateMachine = gameStateMachine;

    public void Exit() { }
    
    public Task AsyncEnter() => 
      throw new System.NotImplementedException();
    
    public void Enter()
    {
      _gameStateMachine.Enter<LoadPolicyState>();
    }
  }
}