using _Project.Scripts.Service_Base;
using _Project.Scripts.Services._data;

namespace _Project.Scripts.Services._Interfaces
{
  public interface IGameSaveSystem : IService
  {
    GameSaveData Get();
    void Save();
  }
}