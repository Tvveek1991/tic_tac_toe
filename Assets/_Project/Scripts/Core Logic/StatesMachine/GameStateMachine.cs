using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts.Core_Logic.StatesMachine.Interfaces;
using _Project.Scripts.Core_Logic.StatesMachine.States;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Core_Logic.StatesMachine
{
  [UsedImplicitly]
  public class GameStateMachine : IGameStateMachine
  {
    private readonly Dictionary<Type, IExitableState> _states;
    private IExitableState _activeExitableState;

    public GameStateMachine(
      IFactory<LoadGameServicesState> loadGameServicesStateFactory,
      IFactory<LoadPolicyState> loadPolicyStateFactory,
      IFactory<LoadHomeState> loadHomeStateFactory,
      IFactory<LoadGameState> loadGameStateFactory)
    {
      _states = new Dictionary<Type, IExitableState>
      {
        [typeof(LoadGameServicesState)] = loadGameServicesStateFactory.Create(),
        [typeof(LoadPolicyState)] = loadPolicyStateFactory.Create(),
        [typeof(LoadHomeState)] = loadHomeStateFactory.Create(),
        [typeof(LoadGameState)] = loadGameStateFactory.Create()
      };

      SetupStateMachineStates();

      Debug.Log("Game State Machine Initialize");
    }

    public IExitableState GetState(Type type) =>
      _states[type];

    public void Enter<TState>() where TState : class, IState
    {
      IState exitableState = ChangeState<TState>();
      exitableState.Enter();
    }

    public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
    {
      IPayloadedState<TPayload> state = ChangeState<TState>();
      state.Enter(payload);
    }

    public async Task AsyncEnter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
    {
      IPayloadedState<TPayload> state = ChangeState<TState>();
      await state.AsyncEnter(payload);
    }

    public async Task AsyncEnter<TState>() where TState : class, IState
    {
      IState exitableState = ChangeState<TState>();
      await exitableState.AsyncEnter();
    }

    private TState ChangeState<TState>() where TState : class, IExitableState
    {
      _activeExitableState?.Exit();
      TState state = GetState<TState>();
      _activeExitableState = state;
      return state;
    }

    private TState GetState<TState>() where TState : class, IExitableState =>
      _states[typeof(TState)] as TState;

    private void SetupStateMachineStates()
    {
      foreach (var state in _states)
        state.Value.Construct(this);
    }
  }
}