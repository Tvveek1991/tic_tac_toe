using System;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Services.Money
{
  public class MoneyService : IMoneyService/*, IService*/
  {
    public uint MoneyCount { get; private set; }
    public event Action<uint> OnCountChange;

    private readonly IGameSaveSystem _gameSaveSystem;

    public MoneyService(IGameSaveSystem gameSaveSystem)
    {
      _gameSaveSystem = gameSaveSystem;
      MoneyCount = _gameSaveSystem.Get().MoneyCount;
    }

    public void Earn(uint count)
    {
      CheckedMethod(count);
      SaveMoney();
    }

    public void Spend(uint count, Action onSpend)
    {
      if (MoneyCount >= count)
      {
        MoneyCount -= count;
        SaveMoney();
        onSpend?.Invoke();
      }
      else
        Debug.Log("Money isn't enough");
    }

    private void SaveMoney()
    {
      OnCountChange?.Invoke(MoneyCount);
      
      _gameSaveSystem.Get().MoneyCount = MoneyCount;
      _gameSaveSystem.Save();
    }

    private void CheckedMethod(uint count)
    {
      try
      {
        MoneyCount = checked(MoneyCount + count);
      }
      catch (OverflowException e)
      {
        Console.WriteLine("CHECKED and CAUGHT:  " + e);
      }
    }
  }
}