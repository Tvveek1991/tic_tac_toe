using System;
using System.Collections.Generic;
using _Project.Scripts.Features.NotLoadedWindow;
using _Project.Scripts.Services._data;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using AppacheAds.Scripts.Interfaces;
using AppacheRemote.Scripts.Interfaces;
using UnityEngine;

namespace _Project.Scripts.Services.Unlocker
{
  public class UnlockService : IUnlockService
  {
    private const string UnlocksDataPath = "Data/Unlocked/Data Unlock Base";

    private readonly IAdsService _adsService;
    private readonly IRemote _remote;
    private readonly IMoneyService _moneyService;
    private readonly IAssetProvider _assetProvider;
    private readonly ILocaleSystem _localeSystem;

    private readonly DataUnlockBase _dataUnlockBase;
    private readonly Dictionary<UnlockType, UnlockHandler> _dictionary;

    private uint CostMultiplier => (uint)(_remote.ShouldEnableFeatureTest() ? 30 : 1);

    private delegate void UnlockHandler(string guid, uint cost, Action onSpend, Action<UnlockType> onFail);

    public UnlockService(IAssetProvider assetProvider, ILocaleSystem localeSystem, IMoneyService moneyService, IAdsService adsService, IRemote remote)
    {
      _localeSystem = localeSystem;
      _assetProvider = assetProvider;
      _adsService = adsService;
      _remote = remote;
      _moneyService = moneyService;

      _dataUnlockBase = assetProvider.GetResource<DataUnlockBase>(UnlocksDataPath);

      if (remote.ShouldEnableFeatureTest())
      {
        _dictionary = new Dictionary<UnlockType, UnlockHandler>
        {
          {UnlockType.Money, SpendMoney},
          {UnlockType.Advertising, SpendMoney}, // !!!
          {UnlockType.Free, OpenFree},
          {UnlockType.IAP, OpenIAP},
        };
      }
      else
      {
        _dictionary = new Dictionary<UnlockType, UnlockHandler>
        {
          {UnlockType.Money, SpendMoney},
          {UnlockType.Advertising, ShowReward},
          {UnlockType.Free, OpenFree},
          {UnlockType.IAP, OpenIAP},
        };
      }
    }

    public void Unlock(string guid, Action onSpend, Action<UnlockType> onFail = null)
    {
      UnlockData unlocking = _dataUnlockBase.GetUnlockData(guid);

      if (unlocking == null)
      {
        Debug.LogError("Item didn't find");
        return;
      }

      _dictionary[unlocking.Type]?.Invoke(guid, unlocking.Cost * CostMultiplier, onSpend, onFail);
    }

    public UnlockType GetUnlockType(string guid)
    {
      UnlockData unlocking = _dataUnlockBase.GetUnlockData(guid);
      return unlocking != null ? unlocking.Type : UnlockType.Free;
    }

    public uint GetCost(string guid) =>
      _dataUnlockBase.GetUnlockData(guid).Cost * CostMultiplier;

    private void SpendMoney(string guid, uint cost, Action onSpend, Action<UnlockType> onFail = null) =>
      _moneyService.Spend(cost, onSpend);

    private void ShowReward(string guid, uint cost, Action onSpend, Action<UnlockType> onFail = null) =>
      _adsService.Rewarded.ShowReward(guid, () => onSpend(), () =>
      {
        onFail?.Invoke(UnlockType.Advertising);
        
        INotLoadedWindowLogic notLoadedWindowLogic = new NotLoadedWindowLogic(_localeSystem, _assetProvider);
        notLoadedWindowLogic.Open();
      });

    private void OpenIAP(string guid, uint cost, Action onSpend, Action<UnlockType> onFail = null) => 
      onSpend?.Invoke();

    private void OpenFree(string guid, uint cost, Action onSpend, Action<UnlockType> onFail = null) =>
      onSpend?.Invoke();
  }
}