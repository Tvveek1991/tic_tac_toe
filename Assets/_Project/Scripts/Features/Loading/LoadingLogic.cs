using _Project.Scripts.Services._Interfaces;

namespace _Project.Scripts.Features.Loading
{
  public class LoadingLogic : ILoadingLogic
  {
    private readonly ILoadingFactory _loadingFactory;

    public LoadingLogic(IAssetProvider assetProvider) => 
      _loadingFactory = new LoadingFactory(assetProvider);

    public void Enter() => 
      _loadingFactory.CreateScreen();

    public void Exit() => 
      _loadingFactory.ClearScreen();
  }
}