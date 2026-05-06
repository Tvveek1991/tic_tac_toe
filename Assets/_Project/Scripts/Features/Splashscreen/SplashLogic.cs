using System;
using _Project.Scripts.Services._Interfaces;

namespace _Project.Scripts.Features.Splashscreen
{
  public class SplashLogic
  {
    private readonly ISplashFactory _splashFactory;
    private readonly Splash _splash;

    public SplashLogic(IAssetProvider assetProvider)
    {
      _splashFactory = new SplashFactory(assetProvider);
      _splash = _splashFactory.CreateSplash();
    }

    public void Launch(Action onEnd)
    {
      _splash.Construct(() =>
      { onEnd?.Invoke(); });
      
      _splash.Launch();
    }
    
    public void Complete() => 
      _splash.Complete();

    public void Clear() => 
      _splashFactory.Clear();
  }
}