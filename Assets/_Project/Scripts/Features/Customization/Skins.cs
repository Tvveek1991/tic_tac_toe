using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Features.Customization.Interface;
using _Project.Scripts.Features.Customization.Mono;
using _Project.Scripts.Features.Customization.Serialized;
using _Project.Scripts.Features.DisableAds;
using _Project.Scripts.Services._Interfaces;
using AppacheAds.Scripts.Interfaces;
using AppacheAnalytics.Scripts.Interfaces;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Features.Customization
{
  public class Skins : ISkins
  {
    private readonly ISkinData[] _skinDatas;
    private SkinsView _skinsView;

    public Action<ISkinData> OnSkinActivated { get; set; }
    public Action<ISkinData> OnSkinReceived { get; set; }

    public Skins(ISkinsFactory skinsFactory, IMoneyService moneyService, IAdsService ads, IAudioService audioService,
      ICurrentScreen screenManager, DisableAdsLogic disableAdsLogic, Action onClickBack)
    {
      _skinDatas = skinsFactory.GetSkinsConfig().SkinsData;

      InstantiateSkinsView();

      void InstantiateSkinsView()
      {
        _skinsView = Object.FindObjectOfType<SkinsView>();
          
        if(_skinsView == null)
          _skinsView = skinsFactory.CreateSkinsView();

        _skinsView.Initialize(this, skinsFactory, moneyService, ads, audioService, screenManager, OnSkinActivatedInvoker, 
          OnSkinReceivedInvoker, disableAdsLogic, onClickBack);
        _skinsView.gameObject.SetActive(false);
      }
    }

    public void AchieveAllSkins()
    {
      foreach (ISkinData skinData in _skinDatas) 
        OnSkinReceivedInvoker(skinData);
      
      if(_skinsView)
        foreach (SkinPanel skinPanel in _skinsView.GetComponentsInChildren<SkinPanel>(true))
          skinPanel.ChangeStatusToReceive();
    }

    public ISkinData[] GetSkinDatasByType(SkinType type)
    {
      List<ISkinData> newSkinDatas = _skinDatas.Where(s => s.GetSkinType() == type).ToList();
      return newSkinDatas.ToArray();
    }

    public void Show() =>
      _skinsView.Show();

    public void Clean()
    {
      OnSkinActivated = null;
      OnSkinReceived = null;
      
      Object.Destroy(_skinsView.gameObject);
    }

    private void OnSkinActivatedInvoker(ISkinData skinData) =>
      OnSkinActivated?.Invoke(skinData);

    private void OnSkinReceivedInvoker(ISkinData skinData)
    {
      ISkinDataAction skinDataAction = (ISkinDataAction)skinData;
      skinDataAction.Receive();

      OnSkinReceived?.Invoke(skinData);
    }
  }
}