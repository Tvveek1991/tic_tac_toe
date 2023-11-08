using _Project.Scripts.Core_Logic.Screens;
using _Project.Scripts.Core_Logic.StatesMachine.Interfaces;
using _Project.Scripts.Core_Logic.StatesMachine.States;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.DisableAds;
using _Project.Scripts.Features.Policy;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Unlocker;
using SableGamesAds.Scripts.Interfaces;
using SableGamesAnalytics.Scripts.Interfaces;
using SableGamesRemote.Scripts.Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.HomeScreen
{
  public class HomeLogic
  {
    private const string GameDataPath = "Data/Game";
    
    private readonly IHomeFactory _homeFactory;
    private readonly IUiFactory _uiFactory;

    private readonly IRemote _remote;
    private readonly IAnalytics _analytics;
    private readonly IAdsService _adsService;
    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;
    private readonly IInAppService _inAppService;
    private readonly IUnlockService _unlockService;
    private readonly IAssetProvider _assetProvider;
    private readonly ICurrentScreen _currentScreen;
    private readonly IGameSaveSystem _gameSaveSystem;
    private readonly IGameStateMachine _gameStateMachine;

    private readonly GameCustomData[] _gameCustomsData;
    
    private DisableAdsLogic _disableAdsLogic;

    private string _ipAddress;
    
    public HomeLogic(IGameStateMachine gameStateMachine, IAssetProvider assetProvider, ICurrentScreen currentScreen,
      IGameSaveSystem gameSaveSystem, IAdsService adsService, IAnalytics analytics, ILocaleSystem localeSystem, IAudioService audioService, 
      IRemote remote, IGameMetaProvider gameMetaProvider, IInAppService inAppService, IUnlockService unlockService)
    {
      _remote = remote;
      _analytics = analytics;
      _adsService = adsService;
      _localeSystem = localeSystem;
      _audioService = audioService;
      _inAppService = inAppService;
      _unlockService = unlockService;
      _assetProvider = assetProvider;
      _currentScreen = currentScreen;
      _gameSaveSystem = gameSaveSystem;
      _gameStateMachine = gameStateMachine;

      _homeFactory = new HomeFactory(assetProvider, localeSystem, audioService, gameMetaProvider);
      _uiFactory = new UiFactory(assetProvider);
      
      _gameCustomsData = assetProvider.GetAllResources<GameCustomData>(GameDataPath);
    }
    
    public void CreateHomeView()
    {
      _currentScreen.SetCurrentScreen(ScreenNames.Home);
      
      UnlockFree();

      _homeFactory.CreateHomeView();
      _homeFactory.HomeViewLogic.Construct(OpenSettingsWindow, ToShop, ToGame, ToNoAds);

      IPrivacyPolicyFactory privacyPolicyFactory = new PrivacyPolicyFactory(_assetProvider);

      _uiFactory.SpawnSettingsWindow();
      _uiFactory.SettingsWindow.Construct(_gameStateMachine, _gameSaveSystem, _audioService, _localeSystem,
        _analytics, privacyPolicyFactory, _currentScreen, _remote);

      CheckNetwork();

      _disableAdsLogic = new DisableAdsLogic(_assetProvider, _gameSaveSystem, _analytics, _adsService, _localeSystem, _inAppService, _audioService, delegate { Debug.Log("OnDisable"); },
        delegate { Debug.Log("OnSwitchDisableAds"); });
      
      _inAppService.AddInAppCallback(_disableAdsLogic);
      
      _adsService.Banner.HideBanner();
    }

    public void CloseHomeView()
    {
      _disableAdsLogic?.Cleanup();

      _homeFactory.ClearHomeView();
      _uiFactory.ClearSettingsWindow();
    }

    private void CheckNetwork()
    {
      if (Application.internetReachability != NetworkReachability.NotReachable) 
        return;

      _uiFactory.SpawnNoInternetConnection();
      _uiFactory.NoInternetWindow.Construct(_localeSystem, () => { _uiFactory.ClearNoInternetWindow(); });
    }

    private void ToShop()
    {
      _adsService.CurrentInterstitial.ShowInterstitial(ScreenNames.Game,
        () => _gameStateMachine.Enter<LoadShopState>(), () => _gameStateMachine.Enter<LoadShopState>());
    }

    private void ToGame()
    {
      // _gameMetaProvider.ClearScore();
      
      _adsService.CurrentInterstitial.ShowInterstitial(ScreenNames.Game,
        () => _gameStateMachine.Enter<LoadGameState>(),
        () => _gameStateMachine.Enter<LoadGameState>());
    }

    private void ToNoAds()
    {
      Debug.Log("Open iap shop");
      _disableAdsLogic.Open();
    }

    private void OpenSettingsWindow()
    {
      _analytics.Send("btn_setting");
      _uiFactory.SettingsWindow.OpenWindow();
    }
    
    private void UnlockFree()
    {
      foreach (GameCustomData gameCustomData in _gameCustomsData)
      {
        if (IsUnlocked(gameCustomData.Id) || _unlockService.GetUnlockType(gameCustomData.Id) != UnlockType.Free) 
          continue;
        
        _gameSaveSystem.Get().UnlockedShopContent.Add(gameCustomData.Id);
        _gameSaveSystem.Get().ChosenShopContent.Add(gameCustomData.Id);
      }
    }
    
    private bool IsUnlocked(string itemId) =>
      _gameSaveSystem.Get().UnlockedShopContent.Contains(itemId);
  }
}