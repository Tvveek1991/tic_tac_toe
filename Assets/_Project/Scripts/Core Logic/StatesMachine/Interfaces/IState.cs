using System.Threading.Tasks;

namespace _Project.Scripts.Core_Logic.StatesMachine.Interfaces
{
  public interface IExitableState
  {
    void Construct(IGameStateMachine gameStateMachine);
    void Exit();
  }

  public interface IState : IExitableState
  {
    Task AsyncEnter();
    void Enter();
  }

  public interface IPayloadedState<TPayload> : IExitableState
  {
    Task AsyncEnter(TPayload gameLogic);
    void Enter(TPayload gameLogic);
  }
}