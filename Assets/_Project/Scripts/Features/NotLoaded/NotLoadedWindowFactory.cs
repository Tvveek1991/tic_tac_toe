using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.NotLoaded
{
  public class NotLoadedWindowFactory : INotLoadedWindowFactory
  {
    private const string NotLoadedWindowPath = "Features/Other/Not Loaded Window";
    private const string BaseCanvas = "Canvas";

    private readonly IAssetProvider _assetProvider;
    
    private NotLoadedWindow _loseScreen;

    public NotLoadedWindowFactory(IAssetProvider assetProvider) => 
      _assetProvider = assetProvider;

    public NotLoadedWindow SpawnNotLoadedWindow()
    {
      Transform parent = GameObject.Find(BaseCanvas).transform;

      var prefab = _assetProvider.GetResource<NotLoadedWindow>(NotLoadedWindowPath);
      _loseScreen = Object.Instantiate(prefab, parent);
      
      return _loseScreen;
    }

    public void Clear()
    {
      if(_loseScreen)
        Object.Destroy(_loseScreen.gameObject);
    }
  }
}