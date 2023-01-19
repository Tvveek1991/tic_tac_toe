using System.Threading.Tasks;
using System;

namespace _Project.Scripts.Core_Logic.StatesMachine.Interfaces
{
  public interface IGameStateMachine
  {
    void Enter<TState>() where TState : class, IState;
    void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>;
    Task AsyncEnter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>;
    Task AsyncEnter<TState>() where TState : class, IState;
    IExitableState GetState(Type type);
  }
}