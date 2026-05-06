using _Project.Scripts.Core_Logic.StatesMachine;
using _Project.Scripts.Core_Logic.StatesMachine.Interfaces;
using _Project.Scripts.Core_Logic.StatesMachine.States;
using Zenject;

namespace _Project.Scripts.Core_Logic.Zenject
{
  public class StatesRegistration : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.Bind<IGameStateMachine>().To<GameStateMachine>().AsSingle();
      Container.BindIFactory<LoadStartupState>().ToSelf().AsSingle();
      Container.BindIFactory<LoadPolicyState>().ToSelf().AsSingle();
      Container.BindIFactory<LoadHomeState>().ToSelf().AsSingle();
      Container.BindIFactory<LoadGameState>().ToSelf().AsSingle();
      Container.BindIFactory<LoadShopState>().ToSelf().AsSingle();
    }
  }
}