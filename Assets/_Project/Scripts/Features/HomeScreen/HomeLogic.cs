using _Project.Scripts.Core_Logic.Screens;
using _Project.Scripts.Core_Logic.StatesMachine.States;
using _Project.Scripts.Features.HomeScreen.Mono;
using _Project.Scripts.Features.Policy;
using _Project.Scripts.Logic.StatesMachine.Interfaces;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Money.Mono;
using AppacheAds.Scripts.Interfaces;
using AppacheAnalytics.Scripts.Interfaces;
using AppacheRemote.Scripts.Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.HomeScreen
{
  public class HomeLogic
  {
    private readonly IHomeFactory _homeFactory;
    private readonly IUiFactory _uiFactory;

    private readonly IRemote _remote;
    private readonly IAnalytics _analytics;
    private readonly IAdsService _adsService;
    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;
    private readonly IMoneyService _moneyService;
    private readonly IAssetProvider _assetProvider;
    private readonly ICurrentScreen _currentScreen;
    private readonly IGameSaveSystem _gameSaveSystem;
    private readonly IGameStateMachine _gameStateMachine;

    private MoneyCounter MoneyCounter { get; set; }

    public HomeLogic(IGameStateMachine gameStateMachine, IAssetProvider assetProvider, ICurrentScreen currentScreen,
      IGameSaveSystem gameSaveSystem, IAdsService adsService, IAnalytics analytics,
      ILocaleSystem localeSystem, IAudioService audioService, IRemote remote, IMoneyService moneyService)
    {
      _remote = remote;
      _analytics = analytics;
      _adsService = adsService;
      _localeSystem = localeSystem;
      _audioService = audioService;
      _moneyService = moneyService;
      _assetProvider = assetProvider;
      _currentScreen = currentScreen;
      _gameSaveSystem = gameSaveSystem;
      _gameStateMachine = gameStateMachine;

      _homeFactory = new HomeFactory(assetProvider);
      _uiFactory = new UiFactory(assetProvider);
    }

    public void CreateHomeView()
    {
      _currentScreen.SetCurrentScreen(ScreenNames.Home);

      HomeView homeScreen = _homeFactory.CreateHomeView();
      homeScreen.Construct(_localeSystem, OpenSettingsWindow, ToShop, ToGame, ToGameMode, ToIapShop, ToNoAds);

      CreateSounds(homeScreen);

      IPrivacyPolicyFactory privacyPolicyFactory = new PrivacyPolicyFactory(_assetProvider);

      _uiFactory.SpawnSettingsWindow();
      _uiFactory.SettingsWindow.Construct(_gameStateMachine, _gameSaveSystem, _audioService, _localeSystem,
        _analytics, privacyPolicyFactory, _currentScreen, _adsService, _remote);

      CheckNetwork();
      
      MoneyCounter = _uiFactory.SpawnMoneyCounter();
      MoneyCounter.Construct(_moneyService);
    }

    private void CreateSounds(Mono.HomeView homeView)
    {
      new ButtonSound(_audioService, homeView.ShopButton.gameObject, AudioId.SoundButton);
      new ButtonSound(_audioService, homeView.NoAdsButton.gameObject, AudioId.SoundButton);
      new ButtonSound(_audioService, homeView.IapShopButton.gameObject, AudioId.SoundButton);
      new ButtonSound(_audioService, homeView.SettingsButton.gameObject, AudioId.SoundButton);
      new ButtonSound(_audioService, homeView.ModeButton.gameObject, AudioId.SoundButton);
      new ButtonSound(_audioService, homeView.StartButton.gameObject, AudioId.SoundButton);
    }

    private void CheckNetwork()
    {
      if (Application.internetReachability != NetworkReachability.NotReachable) 
        return;

      /*_uiFactory.SpawnNoInternetConnection();
      _uiFactory.NoInternetWindow.Construct(_localeSystem, _audioService, () => { _uiFactory.ClearNoInternetWindow(); });*/
    }

    public void CloseHomeView()
    {
      Object.Destroy(MoneyCounter.gameObject);
      
      _homeFactory.ClearHomeView();
      _uiFactory.ClearSettingsWindow();
    }

    private void ToShop()
    {
      // _gameStateMachine.Enter<LoadShopState>();
      /*_adsService.CurrentInterstitial.ShowInterstitial(ScreenNames.Shop,
        _gameStateMachine.Enter<LoadShopState>,
        _gameStateMachine.Enter<LoadShopState>);*/
    }

    private void ToGame()
    {
      _gameStateMachine.Enter<LoadGameState>();
      /*_adsService.CurrentInterstitial.ShowInterstitial(ScreenNames.Game,
        _gameStateMachine.Enter<LoadGameState>,
        _gameStateMachine.Enter<LoadGameState>);*/
    }

    private void ToGameMode()
    {
      // _gameStateMachine.Enter<LoadModeState>();
      /*_adsService.CurrentInterstitial.ShowInterstitial(ScreenNames.Mode,
        _gameStateMachine.Enter<LoadModeState>,
        _gameStateMachine.Enter<LoadModeState>);*/
    }

    private void ToIapShop()
    {
      Debug.Log("Open iap shop");
    }

    private void ToNoAds()
    {
      Debug.Log("Open iap shop");
    }

    private void OpenSettingsWindow()
    {
      _analytics.Send("btn_setting");
      _uiFactory.SettingsWindow.OpenWindow();
    }
  }
}