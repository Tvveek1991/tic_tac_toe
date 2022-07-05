using _Project.Scripts.Logic.StatesMachine;
using _Project.Scripts.Logic.StatesMachine.Interfaces;
using _Project.Scripts.Logic.StatesMachine.States;
using Zenject;

namespace _Project.Scripts.Logic.Zenject
{
  public class StatesRegistration : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.Bind<IGameStateMachine>().To<GameStateMachine>().AsSingle();
      Container.BindIFactory<LoadGameServicesState>().ToSelf().AsSingle();
    }
  }
}