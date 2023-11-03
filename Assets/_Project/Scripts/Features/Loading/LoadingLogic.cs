using System;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;

namespace _Project.Scripts.Features.Loading
{
  public class LoadingLogic : ILoadingLogic
  {
    private readonly ILoadingFactory _loadingFactory;

    public LoadingLogic(IAssetProvider assetProvider, IGameMetaProvider gameMetaProvider, ILocaleSystem localeSystem) => 
      _loadingFactory = new LoadingFactory(assetProvider, gameMetaProvider, localeSystem);

    public void Enter(Action onEnd) => 
      _loadingFactory.CreateScreen(() =>
      {
        onEnd?.Invoke();
        Exit();
      });

    public void Exit() => 
      _loadingFactory.ClearScreen();
  }
}