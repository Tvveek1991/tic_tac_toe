using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Animation;
using _Project.Scripts.Animations.UI;
using _Project.Scripts.Core_Logic.Screens;
using _Project.Scripts.Core_Logic.StatesMachine.Interfaces;
using _Project.Scripts.Features.Policy;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio.Enum;
using _Project.Scripts.Services.Audio.Executor;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Localization.Mono;
using DG.Tweening;
using SableGamesAds.Scripts.Interfaces;
using SableGamesAnalytics.Scripts.Interfaces;
using SableGamesRemote.Scripts.Interfaces;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.Settings
{
  public class Settings : MonoBehaviour
  {
    private const string IconMusicOnPath = "icon_music_on";
    private const string IconMusicOffPath = "icon_music_off";
    private const string IconSoundOnPath = "icon_sound_on";
    private const string IconSoundOffPath = "icon_sound_off";
    private const string IconVibrationOffPath = "icon_vibration_off";
    private const string IconVibrationOnPath = "icon_vibration_on";

    private const float SwitcherTime = .25f;

    public Image Background;

    public Button ButtonBack;
    public Button ButtonPrivacyPolicy;
    public Button ButtonRate;
    public Button ButtonGallery;

    public Button ButtonMusic;
    public Button ButtonSound;

    public Button Screen;

    public Image IconMusic;
    public Image IconSound;
    public Transform SwitcherMusic;
    public Transform SwitcherSound;

    public Sprite Active;
    public Sprite Inactive;
    public Color InactiveColor;

    private Sprite _musicOn;
    private Sprite _musicOff;
    private Sprite _soundOn;
    private Sprite _soundOff;
    private Sprite _switcherOn;
    private Sprite _switcherOff;

    private IGameSaveSystem _gameSaveSystem;
    private IAudioService _audioService;
    private IAnalytics _analytics;
    private ICurrentScreen _screenManager;

    private PrivacyPolicy _privacyPolicy;
    private IPrivacyPolicyFactory _privacyPolicyFactory;
    
    private IGameStateMachine _gameStateMachine;

    private List<FadeCanvasGroup> _fadeCanvasGroups;
    private SlideWindow _slideWindow;

    private string _cachedScreen;
    private IAdsService _adsService;
    private IRemote _remote;

    public void Construct(
      IGameStateMachine gameStateMachine,
      IGameSaveSystem gameSaveSystem,
      IAudioService audioService,
      ILocaleSystem localeSystem,
      IAnalytics analytics,
      IPrivacyPolicyFactory privacyPolicyFactor,
      ICurrentScreen screenManager,
      IAdsService adsService,
      IRemote remote)
    {
      _remote = remote;
      _adsService = adsService;
      _screenManager = screenManager;
      _gameStateMachine = gameStateMachine;

      _gameSaveSystem = gameSaveSystem;
      _audioService = audioService;
      _analytics = analytics;

      _privacyPolicyFactory = privacyPolicyFactor;
      _privacyPolicy = _privacyPolicyFactory.SpawnPrivacyPolicy();
      _privacyPolicy.Construct(_gameSaveSystem, _audioService, localeSystem, analytics, _remote);

      Localize(localeSystem);
      LoadSprites();
      AssignListeners();
      AssignFades();

      MuteMusic(_gameSaveSystem.Get().IsMusicDisabled);
      MuteSounds(_gameSaveSystem.Get().IsSoundDisabled);

      gameObject.SetActive(false);

      new ButtonAnimation(ButtonBack.transform);

      _slideWindow = new SlideWindow(Background.GetComponent<RectTransform>());

      SoundCreator();
    }

    private void SoundCreator()
    {
      new ButtonSound(_audioService, ButtonBack.gameObject, AudioId.ClickBack);
      new ButtonSound(_audioService, ButtonMusic.gameObject, AudioId.Click);
      new ButtonSound(_audioService, ButtonSound.gameObject, AudioId.Click);
      new ButtonSound(_audioService, ButtonPrivacyPolicy.gameObject, AudioId.Click);
      new ButtonSound(_audioService, ButtonRate.gameObject, AudioId.Click);
      new ButtonSound(_audioService, ButtonGallery.gameObject, AudioId.Click);
    }

    public void OpenWindow()
    {
      _slideWindow.Open();

      gameObject.SetActive(true);

      _fadeCanvasGroups.ToArray().ForEach(x => x.Appear());
    }

    private void Localize(ILocaleSystem localeSystem)
    {
      List<LocalizedMainText> localizedTexts = transform.GetComponentsInChildren<LocalizedMainText>(true).ToList();
      localizedTexts.ForEach(item =>
      {
        item.Construct(localeSystem);
        item.Localize();
      });
    }

    private void CloseWindow()
    {
      _slideWindow.Close(null, () =>
      {
        gameObject.SetActive(false);
      });

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
      ButtonBack.onClick.AddListener(Back);
      ButtonPrivacyPolicy.onClick.AddListener(PrivacyPolicy);
      ButtonMusic.onClick.AddListener(SwitchMusic);
      ButtonSound.onClick.AddListener(SwitchSound);
      Screen.onClick.AddListener(Back);
    }

    private void AssignFades()
    {
      _fadeCanvasGroups = new List<FadeCanvasGroup>();
      foreach (CanvasGroup canvasGroup in GetComponentsInChildren<CanvasGroup>(true))
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
          IconMusic.sprite = mute ? _musicOff : _musicOn;
          SwitcherMusic.DOLocalMove(mute ? Vector3.left * 19 : Vector3.right * 19, SwitcherTime); //ToDo: Заменить на бахину анимацию
          var music = SwitcherMusic.parent.GetComponent<Image>();
          music.sprite = mute ? Inactive : Active;
          music.color = mute ? InactiveColor : Color.white;
          _audioService.MuteMusic(mute);
          break;
        case SoundType.Sound:
          IconSound.sprite = mute ? _soundOff : _soundOn;
          SwitcherSound.DOLocalMove(mute ? Vector3.left * 19 : Vector3.right * 19, SwitcherTime); //ToDo: Заменить на бахину анимацию
          var sound = SwitcherSound.parent.GetComponent<Image>();
          sound.sprite = mute ? Inactive : Active;
          sound.color = mute ? InactiveColor : Color.white;
          _audioService.MuteSounds(mute);
          break;
      }
    }

    private enum SoundType
    {
      Music,
      Sound
    }

    
    public void Clear() => 
      _privacyPolicyFactory.Clear();
  }
}