using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts.Logic.StatesMachine.Interfaces;
using _Project.Scripts.Logic.StatesMachine.States;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Logic.StatesMachine
{
  [UsedImplicitly]
  public class GameStateMachine : IGameStateMachine
  {
    private readonly Dictionary<Type, IExitableState> _states;
    private IExitableState _activeExitableState;

    public GameStateMachine(IFactory<LoadGameServicesState> loadGameServicesStateFactory)
    {
      _states = new Dictionary<Type, IExitableState>
      {
        [typeof(LoadGameServicesState)] = loadGameServicesStateFactory.Create()
      };
      
      SetupStateMachineStates();

      Debug.Log("Game State Machine Initialize");
    }

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