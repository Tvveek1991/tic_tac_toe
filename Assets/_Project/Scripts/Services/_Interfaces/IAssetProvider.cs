using System.Threading.Tasks;
using _Project.Scripts.Service_Base;
using UnityEngine;

namespace _Project.Scripts.Services._Interfaces
{
  public interface IAssetProvider : IService
  {
    T GetResource<T>(string path) where T : Object;
    T[] GetAllResources<T>(string path) where T : Object;
  }
}