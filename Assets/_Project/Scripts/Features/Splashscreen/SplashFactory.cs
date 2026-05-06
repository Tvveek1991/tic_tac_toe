using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.Splashscreen
{
  public class SplashFactory : ISplashFactory
  {
    private const string SplashPath = "Features/Splashscreen/Logo Screen";
    
    private readonly IAssetProvider _assetProvider;
    private Splash _splash;

    public SplashFactory(IAssetProvider assetProvider)
    {
      _assetProvider = assetProvider;
    }
    
    public Splash CreateSplash()
    {
      var splashPrefab = _assetProvider.GetResource<GameObject>(SplashPath);
      _splash = Object.Instantiate(splashPrefab).GetComponent<Splash>();
      return _splash;
    }

    public void Clear()
    {
      if(_splash)
        Object.Destroy(_splash.gameObject);
    }
  }
}