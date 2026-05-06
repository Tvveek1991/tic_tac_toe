using System;
using _Project.Scripts.Services._Interfaces;

namespace _Project.Scripts.Features.Loading
{
  public class LoadingLogic : ILoadingLogic
  {
    private readonly ILoadingFactory _loadingFactory;

    public LoadingLogic(IAssetProvider assetProvider) => 
      _loadingFactory = new LoadingFactory(assetProvider);

    public void Enter(Action onEnd) => 
      _loadingFactory.CreateScreen(() =>
      {
        onEnd?.Invoke();
      });

    public void Exit() => 
      _loadingFactory.ClearScreen();
  }
}