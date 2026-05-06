using System;
using _Project.Scripts.Services.Unlocker;

namespace _Project.Scripts.Services._Interfaces
{
  public interface IUnlockService
  {
    public void Unlock(string guid, Action onSpend, Action<UnlockType> onFail = null);
    public UnlockType GetUnlockType(string guid);
    public uint GetCost(string guid);
  }
}