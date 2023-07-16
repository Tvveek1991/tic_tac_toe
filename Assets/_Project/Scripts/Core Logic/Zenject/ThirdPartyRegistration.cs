using SableGamesAds;
using SableGamesAds.Scripts.Interfaces;
using SableGamesAnalytics.Scripts.Interfaces;
using SableGamesRemote;
using SableGamesRemote.Scripts.Interfaces;
using Zenject;

namespace _Project.Scripts.Core_Logic.Zenject
{
  public class ThirdPartyRegistration : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.Bind<IAnalytics>().To<SableGamesAnalytics.Analytics>().AsSingle();
      Container.Bind<IRemote>().To<Remote>().AsSingle();
      Container.Bind<IAdsService>().To<AdsService>().AsSingle();
    }
  }
}