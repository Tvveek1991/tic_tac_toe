using System.Collections.Generic;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Features.HomeScreen
{
  public class HomeFactory : IHomeFactory
  {
    private const string HomePath = "Features/HomeScreen/Home";

    private const string BaseCanvas = "Canvas";

    private readonly IAssetProvider _assetProvider;

    public Mono.HomeScreen HomeScreen { get; private set; }

    public HomeFactory(IAssetProvider assetProvider)
    {
      _assetProvider = assetProvider;
    }

    public Mono.HomeScreen CreateHomeScreen()
    {
      ClearHomeScreen();

      var parent = GameObject.Find(BaseCanvas).transform;

      var homeScreen = _assetProvider.GetResource<Mono.HomeScreen>(HomePath);
      HomeScreen = Object.Instantiate(homeScreen, parent);

      return HomeScreen;
    }

    public void ClearHomeScreen()
    {
      if (!HomeScreen) return;
      
      HomeScreen.Clear();
      Object.Destroy(HomeScreen.gameObject);
    }
  }
}