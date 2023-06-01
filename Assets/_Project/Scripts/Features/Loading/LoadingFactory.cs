using _Project.Scripts.Core_Logic.Common;
using _Project.Scripts.Services._Interfaces;

namespace _Project.Scripts.Features.Loading
{
  public class LoadingFactory : GameObjectFactory, ILoadingFactory
  {
    private const string LoadingPrefabPath = "Screens/Other/Loading";

    public LoadingMono LoadingMono { get; private set; }

    public LoadingFactory(IAssetProvider assetProvider) : base(assetProvider)
    {}

    public void CreateScreen()
    {
      ClearScreen();
      LoadingMono = Create<LoadingMono>(LoadingPrefabPath);
    }

    public void ClearScreen()
    {
      if (LoadingMono != null)
      {
        Clear(LoadingMono.gameObject);
        LoadingMono = null;
      }
    }
  }
}