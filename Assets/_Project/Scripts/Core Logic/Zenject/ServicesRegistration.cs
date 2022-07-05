using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.AssetProvider;
using _Project.Scripts.Services.Coins;
using Zenject;

namespace _Project.Scripts.Logic.Zenject
{
  public class ServicesRegistration : MonoInstaller
  {
    public override void InstallBindings()
    {      
      BindSingleService<IAssetProvider, AssetProvider>();
      BindSingleService<IGameCoins, GameCoins>();
    }

    private void BindSingleService<T, T2>() where T2 : T => 
      Container.Bind<T>().To<T2>().AsSingle();
  }
}