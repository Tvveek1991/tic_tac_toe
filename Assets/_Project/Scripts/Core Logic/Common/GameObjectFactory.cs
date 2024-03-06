using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Core_Logic.Common
{
  public abstract class GameObjectFactory
  {
    private readonly IAssetProvider _assetProvider;

    protected GameObjectFactory(IAssetProvider assetProvider) => 
      _assetProvider = assetProvider;

    protected T Create<T>(string prefabPath) where T : MonoBehaviour
    {
      var prefab = _assetProvider.GetResource<T>(prefabPath);
      T obj = Object.Instantiate(prefab);
      return obj;
    }

    protected T Create<T>(string prefabPath, Transform parent) where T : MonoBehaviour
    {
      var prefab = _assetProvider.GetResource<T>(prefabPath);
      T obj = Object.Instantiate(prefab, parent);
      return obj;
    }

    protected T Find<T>(string path) where T : Object
    {
      var prefab = _assetProvider.GetResource<T>(path);
      return prefab;
    }

    protected void Clear(GameObject obj)
    {
      if (obj != null)
        Object.Destroy(obj);
    }
  }
}