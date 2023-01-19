using _Project.Scripts.Core_Logic.Common;
using _Project.Scripts.Features.HomeScreen.Mono;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.HomeScreen
{
  public class HomeFactory : GameObjectFactory, IHomeFactory
  {
    private const string BaseCanvas = "Canvas";
    
    private const string HomeViewPath = "Features/HomeScreen/Home";

    public HomeView HomeView { get; private set; }

    public HomeFactory(IAssetProvider assetProvider) : base(assetProvider)
    {
    }

    public void CreateHomeView()
    {
      ClearHomeView();
      HomeView = Create<HomeView>(HomeViewPath, GameObject.Find(BaseCanvas).transform);
    }

    public void ClearHomeView()
    {
      if (!HomeView) return;
      
      Object.Destroy(HomeView.gameObject);
    }
  }
}