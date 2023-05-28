using System;
using System.Collections.Generic;
using _Project.Scripts.Core_Logic.Screens;
using _Project.Scripts.Core_Logic.StatesMachine.Interfaces;
using _Project.Scripts.Core_Logic.StatesMachine.States;
using _Project.Scripts.Features.Splashscreen;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Network;
using AppacheAds.Scripts.Interfaces;
using AppacheAnalytics.Scripts.Interfaces;
using AppacheRemote.Scripts.Interfaces;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Core_Logic
{
  public class GameBootstrapper : MonoBehaviour
  {
    private IAssetProvider _assetProvider;
    private ICurrentScreen _screenManager;

    private IAnalytics _analytics;
    private IRemote _remote;
    private IAdsService _adsService;

    private IGameStateMachine _gameStateMachine;
    private SplashLogic _splashLogic;

    private void Awake()
    {
      Application.targetFrameRate = 120;
      LaunchWithSplash(LaunchGameStateMachine);
    }

    [Inject]
    public void Construct(IAssetProvider assetProvider,
      IRemote remote,
      IAdsService adsService,
      IAnalytics analytics,
      ICurrentScreen screenManager,
      IGameStateMachine gameStateMachine)
    {
      _adsService = adsService;
      _remote = remote;
      _assetProvider = assetProvider;
      _analytics = analytics;
      _gameStateMachine = gameStateMachine;
      
      _screenManager = screenManager;
      _screenManager.SetCurrentScreen(ScreenNames.Splash);

    }

    private void LaunchWithSplash(Action initializeGame)
    {
      _splashLogic = new SplashLogic(_assetProvider);
      _splashLogic.Launch(initializeGame);

      InitializeRemote();
    }

    private void InitializeRemote()
    {
      _remote.OnFetch += InitializeAnalytics;
      _remote.Initialize();
    }

    private void InitializeAnalytics()
    {
      _analytics.OnInitialize += InitializeAds;
      _analytics.Initialize();
    }

    private void InitializeAds()
    {
      SendRemoteEvents();
      
      _adsService.Initialize(_remote.GetPlacementType(), () =>
      {
#if !WITHOUT_BANNER
        _adsService.Banner.OnBannerLoad += () => { _adsService.Banner.HideBanner(); };
        _adsService.Banner.HideBanner();
#endif
        _splashLogic.Complete();
      });
    }

    private void SendRemoteEvents()
    {
      // Flurry.UserProperties.Add("AB_remote_config", _remote.GetPlacementType().ToString());

      _analytics.Send("AB_remote_config", new Dictionary<string, object>()
      {
        {"group", _remote.GetPlacementType()}
      }, true);

      if (_analytics.IsFirstLaunch)
        _analytics.Send($"v{Application.version.Replace(".", "_")}", true);
    }

    private void LaunchGameStateMachine()
    {
      _adsService.CurrentInterstitial.ShowInterstitial("start",
        _gameStateMachine.Enter<LoadGameServicesState>,
        _gameStateMachine.Enter<LoadGameServicesState>);
    }
  }
}