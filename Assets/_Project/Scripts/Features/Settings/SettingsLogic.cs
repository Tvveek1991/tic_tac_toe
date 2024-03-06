using System.Collections.Generic;
using _Project.Scripts.Animation;
using _Project.Scripts.Animations.UI;
using _Project.Scripts.Core_Logic.Screens;
using _Project.Scripts.Core_Logic.StatesMachine.Interfaces;
using _Project.Scripts.Core_Logic.StatesMachine.States;
using _Project.Scripts.Features.Policy;
using _Project.Scripts.Features.Settings.Mono;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio.Enum;
using _Project.Scripts.Services.Audio.Executor;
using _Project.Scripts.Services.Localization.Logic;
using DG.Tweening;
using SableGamesAnalytics.Scripts.Interfaces;
using SableGamesRemote.Scripts.Interfaces;
using Sirenix.Utilities;
using UnityEngine;

namespace _Project.Scripts.Features.Settings
{
  public class SettingsLogic
  {
    private const string IconMusicOnPath = "icon_music_on";
    private const string IconMusicOffPath = "icon_music_off";
    private const string IconSoundOnPath = "icon_sound_on";
    private const string IconSoundOffPath = "icon_sound_off";

    private const float SwitcherTime = .25f;

    private readonly IRemote _remote;
    private readonly IAnalytics _analytics;
    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;
    private readonly ICurrentScreen _screenManager;
    private readonly IAssetProvider _assetProvider;
    private readonly IGameSaveSystem _gameSaveSystem;
    private readonly IGameStateMachine _gameStateMachine;

    private readonly SettingsView _settingsView;
    
    private Sprite _musicOn;
    private Sprite _musicOff;
    private Sprite _soundOn;
    private Sprite _soundOff;
    private Sprite _switcherOn;
    private Sprite _switcherOff;
    
    private PrivacyPolicy _privacyPolicy;
    private IPrivacyPolicyFactory _privacyPolicyFactory;

    private List<FadeCanvasGroup> _fadeCanvasGroups;
    private SlideWindow _slideWindow;

    private string _cachedScreen;

    public SettingsLogic(SettingsView settingsView, IGameStateMachine gameStateMachine, IGameSaveSystem gameSaveSystem, IAudioService audioService,
      ILocaleSystem localeSystem, IAnalytics analytics, IAssetProvider assetProvider, ICurrentScreen screenManager, IRemote remote)
    {
      _remote = remote;
      _analytics = analytics;
      _localeSystem = localeSystem;
      _audioService = audioService;
      _assetProvider = assetProvider;
      _screenManager = screenManager;
      _gameSaveSystem = gameSaveSystem;
      _gameStateMachine = gameStateMachine;
      
      _settingsView = settingsView;
    }

    public void Construct()
    {
      CreatePolicy(_localeSystem, _analytics, _assetProvider);

      LoadSprites();
      AssignListeners();
      AssignFades();

      MuteMusic(_gameSaveSystem.Get().IsMusicDisabled);
      MuteSounds(_gameSaveSystem.Get().IsSoundDisabled);
      RefreshHintsSwitcher();
      
      SoundCreator();
      
      new ButtonAnimation(_settingsView.ButtonBack.transform);
      _slideWindow = new SlideWindow(_settingsView.Background);

      _settingsView.gameObject.SetActive(false);
    }

    public void OpenWindow()
    {
      _slideWindow.Open();
      _settingsView.gameObject.SetActive(true);
      _fadeCanvasGroups.ToArray().ForEach(x => x.Appear());
    }
    
    public void Clear() =>
      _privacyPolicyFactory.Clear();

    private void CreatePolicy(ILocaleSystem localeSystem, IAnalytics analytics, IAssetProvider assetProvider)
    {
      _privacyPolicyFactory = new PrivacyPolicyFactory(assetProvider);
      _privacyPolicy = _privacyPolicyFactory.SpawnPrivacyPolicy();
      _privacyPolicy.Construct(_gameSaveSystem, _audioService, localeSystem, analytics, _remote,
        () => { _gameStateMachine.Enter<LoadPolicyState>(); });
    }

    private void SoundCreator()
    {
      new ButtonSound(_audioService, _settingsView.ButtonMusic.gameObject, AudioId.Click);
      new ButtonSound(_audioService, _settingsView.ButtonSound.gameObject, AudioId.Click);
      new ButtonSound(_audioService, _settingsView.ButtonHints.gameObject, AudioId.Click);
      new ButtonSound(_audioService, _settingsView.ButtonRate.gameObject, AudioId.Click);
      new ButtonSound(_audioService, _settingsView.ButtonGallery.gameObject, AudioId.Click);
      new ButtonSound(_audioService, _settingsView.ButtonBack.gameObject, AudioId.ClickBack);
      new ButtonSound(_audioService, _settingsView.ButtonPrivacyPolicy.gameObject, AudioId.Click);
    }

    private void CloseWindow()
    {
      _slideWindow.Close(null, () => { _settingsView.gameObject.SetActive(false); });
      _fadeCanvasGroups.ToArray().ForEach(x => x.Disappear());
    }

    private void Back() =>
      CloseWindow();

    private void PrivacyPolicy()
    {
      _privacyPolicy.ClosingStarted += OnClosingStarted;
      _privacyPolicy.OpenWindow();
      CloseWindow();
      _cachedScreen = _screenManager.GetCurrentScreen();
      _screenManager.SetCurrentScreen(ScreenNames.Privacy);

      void OnClosingStarted()
      {
        OpenWindow();
        _privacyPolicy.ClosingStarted -= OnClosingStarted;
        _screenManager.SetCurrentScreen(_cachedScreen);
      }
    }

    private void SwitchMusic() =>
      MuteMusic(!_audioService.MusicIsMute());

    private void SwitchSound() =>
      MuteSounds(!_audioService.SoundsIsMute());

    private void SwitchHints()
    {
      _gameSaveSystem.Get().IsHintsDisabled = !_gameSaveSystem.Get().IsHintsDisabled;
      _gameSaveSystem.Save();

      RefreshHintsSwitcher();
    }

    private void LoadSprites()
    {
      _musicOn = LoadSprite(IconMusicOnPath);
      _musicOff = LoadSprite(IconMusicOffPath);
      _soundOn = LoadSprite(IconSoundOnPath);
      _soundOff = LoadSprite(IconSoundOffPath);
    }

    private Sprite LoadSprite(string path) =>
      Resources.Load<Sprite>(path);

    private void AssignListeners()
    {
      _settingsView.Screen.onClick.AddListener(Back);
      _settingsView.ButtonBack.onClick.AddListener(Back);
      _settingsView.ButtonMusic.onClick.AddListener(SwitchMusic);
      _settingsView.ButtonSound.onClick.AddListener(SwitchSound);
      _settingsView.ButtonHints.onClick.AddListener(SwitchHints);
      // _settingsView.ButtonPrivacyPolicy.onClick.AddListener(PrivacyPolicy);
    }

    private void AssignFades()
    {
      _fadeCanvasGroups = new List<FadeCanvasGroup>();
      foreach (CanvasGroup canvasGroup in _settingsView.GetComponentsInChildren<CanvasGroup>(true))
        _fadeCanvasGroups.Add(new FadeCanvasGroup(canvasGroup));
    }

    private void MuteMusic(bool mute)
    {
      SwitchAudio(SoundType.Music, mute);

      _gameSaveSystem.Get().IsMusicDisabled = mute;
      _gameSaveSystem.Save();
    }

    private void MuteSounds(bool mute)
    {
      SwitchAudio(SoundType.Sound, mute);

      _gameSaveSystem.Get().IsSoundDisabled = mute;
      _gameSaveSystem.Save();
    }

    private void SwitchAudio(SoundType type, bool mute)
    {
      switch (type)
      {
        case SoundType.Music:
          _settingsView.SwitcherMusic.DOLocalMove(mute ? Vector3.left * 40 : Vector3.right * 40, SwitcherTime);
          _audioService.MuteMusic(mute);
          break;
        case SoundType.Sound:
          _settingsView.SwitcherSound.DOLocalMove(mute ? Vector3.left * 40 : Vector3.right * 40, SwitcherTime);
          _audioService.MuteSounds(mute);
          break;
      }
    }

    private void RefreshHintsSwitcher() =>
      _settingsView.SwitcherHints.DOLocalMove(_gameSaveSystem.Get().IsHintsDisabled ? Vector3.left * 40 : Vector3.right * 40, SwitcherTime);

    private enum SoundType
    {
      Music,
      Sound
    }
  }
}