using _Project.Scripts.Features.HomeScreen.Mono;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.HomeScreen
{
  public class HomeFactory : IHomeFactory
  {
    private const string HomeViewPath = "Features/HomeScreen/Home";

    private const string BaseCanvas = "Canvas";

    private readonly IAssetProvider _assetProvider;

    public HomeView HomeView { get; private set; }

    public HomeFactory(IAssetProvider assetProvider)
    {
      _assetProvider = assetProvider;
    }

    public HomeView CreateHomeView()
    {
      ClearHomeView();

      Transform parent = GameObject.Find(BaseCanvas).transform;

      var homeScreen = _assetProvider.GetResource<HomeView>(HomeViewPath);
      HomeView = Object.Instantiate(homeScreen, parent);

      return HomeView;
    }

    public void ClearHomeView()
    {
      if (!HomeView) return;
      
      Object.Destroy(HomeView.gameObject);
    }
  }
}