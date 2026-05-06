using _Project.Scripts.Service_Base;

namespace _Project.Scripts.Services._Interfaces
{
  public interface IInputResolver : IService
  {
    bool IsLocked { get; }
    void Enable();
    void Disable();
  }
}