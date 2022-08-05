using System;
using System.Collections.Generic;
using _Project.Scripts.Core_Logic.StatesMachine.States;
using _Project.Scripts.Features.Splashscreen;
using _Project.Scripts.Logic.StatesMachine.Interfaces;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Network;
using AppacheAds.Scripts.Interfaces;
using AppacheAnalytics.Scripts.Interfaces;
using AppacheAnalytics.Scripts.Logic;
using AppacheRemote.Scripts.Interfaces;
using FlurrySDK;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Core_Logic
{
  public class GameBootstrapper : MonoBehaviour
  {
    private IAssetProvider _assetProvider;
    private INetworkController _networkController;
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
      INetworkController networkController,
      IGameStateMachine gameStateMachine)
    {
      _adsService = adsService;
      _remote = remote;
      _assetProvider = assetProvider;
      _networkController = networkController;
      _analytics = analytics;
      _gameStateMachine = gameStateMachine;
    }

    private void LaunchWithSplash(Action initializeGame)
    {
      _splashLogic = new SplashLogic(_assetProvider);
      _splashLogic.Launch(initializeGame);

      _remote.OnFetch += InitializeServices;
      _remote.Initialize();
    }

    private void InitializeServices()
    {
      PrewarmServices();

      //ToDo: Засетить в аналитику Splash экран

      InitializeAnalytics();
      InitializeCrashlytics();
      InitializeAds();
      InitializeNetwork();
    }

    private void PrewarmServices()
    {
      _screenManager = new ScreenManager();
    }

    private void InitializeAds()
    {
      _adsService.Initialize(_remote.GetPlacementType(), () => { _splashLogic.Complete(); });
    }

    private void InitializeAnalytics()
    {
      _analytics.Initialize();
      _analytics.OnInitialize += InitializeAnalyticsRemoteEvents;
    }

    private void InitializeAnalyticsRemoteEvents()
    {
      Flurry.UserProperties.Add("AB_remote_config", _remote.GetPlacementType().ToString());

      _analytics.Send("AB_remote_config", new Dictionary<string, object>()
      {
        {"group", _remote.GetPlacementType()}
      }, true);

      if (_analytics.IsFirstLaunch)
        _analytics.Send($"v{Application.version.Replace(".", "_")}", true);
    }

    private void InitializeCrashlytics()
    {
      Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
      {
        var dependencyStatus = task.Result;
        if (dependencyStatus == Firebase.DependencyStatus.Available)
        {
          Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
        }
        else
          Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
      });
    }
    
    private void InitializeNetwork()
    {
      _networkController.LaunchService();
    }

    private void LaunchGameStateMachine()
    {
      _adsService.CurrentInterstitial.ShowInterstitial("start",
        _gameStateMachine.Enter<LoadGameServicesState>,
        _gameStateMachine.Enter<LoadGameServicesState>);
    }
  }
}