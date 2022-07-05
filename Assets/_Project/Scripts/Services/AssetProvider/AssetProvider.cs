using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Services.AssetProvider
{
  public class AssetProvider : IAssetProvider
  {
    public T GetResource<T>(string path) where T : Object =>
      Resources.Load<T>(path);

    public T[] GetAllResources<T>(string path) where T : Object =>
      Resources.LoadAll<T>(path);
  }
}