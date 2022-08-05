using System;

namespace _Project.Scripts.Services._Interfaces
{
  public interface IMoneyService
  {
    public uint MoneyCount {get;}
    public event Action<uint> OnCountChange;
    public void Earn(uint count);
    public void Spend(uint count, Action onUnlock);
  }
}