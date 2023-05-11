using AppacheAds;
using AppacheAds.Scripts.Interfaces;
using AppacheAnalytics.Scripts.Interfaces;
using AppacheRemote;
using AppacheRemote.Scripts.Interfaces;
using Zenject;

namespace _Project.Scripts.Core_Logic.Zenject
{
  public class ThirdPartyRegistration : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.Bind<IAnalytics>().To<AppacheAnalytics.AnalyticsDummy>().AsSingle();
      Container.Bind<IRemote>().To<RemoteDummy>().AsSingle();
      Container.Bind<IAdsService>().To<AdsService>().AsSingle();
    }
  }
}