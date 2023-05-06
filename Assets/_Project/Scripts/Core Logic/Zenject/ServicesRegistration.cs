using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.AssetProvider;
using _Project.Scripts.Services.Audio;
using _Project.Scripts.Services.GameMetaProvider;
using _Project.Scripts.Services.InApps.Scripts;
using _Project.Scripts.Services.InApps.Scripts.Main;
using _Project.Scripts.Services.InputResolver;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Money;
using _Project.Scripts.Services.Network;
using _Project.Scripts.Services.SaveSystem;
using _Project.Scripts.Services.Timer;
using _Project.Scripts.Services.Timer.Interfaces;
using AppacheAnalytics.Scripts.Interfaces;
using AppacheAnalytics.Scripts.Logic;
using Zenject;

namespace _Project.Scripts.Core_Logic.Zenject
{
  public class ServicesRegistration : MonoInstaller
  {
    public override void InstallBindings()
    {
      BindSingleService<IAssetProvider, AssetProvider>();
      BindSingleService<IGameMetaProvider, GameMetaProvider>();
      BindSingleService<IMoneyService, MoneyService>();
      BindSingleService<ILocaleSystem, LocaleSystem>();
      BindSingleService<IAudioService, AudioService>();
      BindSingleService<ICurrentScreen, ScreenManager>();
      BindSingleService<IGameSaveSystem, GameSaveSystem>();
      BindSingleService<ITimerController, TimerController>();
      BindSingleService<IInputResolver, InputResolver>();
      BindSingleService<IInAppService, InAppService>();
      BindSingleService<INetwork, Network>();
    }

    private void BindSingleService<T, T2>() where T2 : T => 
      Container.Bind<T>().To<T2>().AsSingle();
  }
}