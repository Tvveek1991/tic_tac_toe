using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Animation;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Features.HomeScreen.Mono;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio.Enum;
using _Project.Scripts.Services.Audio.Executor;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Localization.Mono;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Features.HomeScreen
{
  public class HomeViewLogic : IHomeViewLogic
  {
    private const float SwitcherTime = .25f;

    private readonly HomeViewMono _homeViewMono;
    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;
    private readonly IGameMetaProvider _gameMetaProvider;

    public HomeViewLogic(HomeViewMono homeViewMono, ILocaleSystem localeSystem, IAudioService audioService, IGameMetaProvider gameMetaProvider)
    {
      _homeViewMono = homeViewMono;
      _localeSystem = localeSystem;
      _audioService = audioService;
      _gameMetaProvider = gameMetaProvider;
    }

    public void Construct(Action openSettingsWindow, Action toShop, Action<bool> toGame, Action toNoAds)
    {
      _homeViewMono.ShopButton.onClick.AddListener(toShop.Invoke);
      _homeViewMono.NoAdsButton.onClick.AddListener(toNoAds.Invoke);
      _homeViewMono.CreateButton.onClick.AddListener(() => toGame.Invoke(true));
      _homeViewMono.ConnectButton.onClick.AddListener(() => toGame.Invoke(false));
      _homeViewMono.SettingsButton.onClick.AddListener(openSettingsWindow.Invoke);

      _homeViewMono.BackButton.onClick.AddListener(() => SetEnemyTypePanel(false));
      _homeViewMono.ScreenButton.onClick.AddListener(() => SetEnemyTypePanel(false));

      _homeViewMono.ClassicButton.onClick.AddListener(() => OnClickGame(Game.Classic));
      _homeViewMono.BuhurtButton.onClick.AddListener(() => OnClickGame(Game.Buhurt));

      _homeViewMono.EnemyTypeButton.onClick.AddListener(() => SwitchEnemy(_gameMetaProvider.GetEnemyType() != EnemyType.OneDevice));
      SwitchEnemy(_gameMetaProvider.GetEnemyType() == EnemyType.OneDevice);

      CreateAnimations();
      CreateSounds();

      Localize(_localeSystem);
    }

    private void Localize(ILocaleSystem localeSystem)
    {
      List<LocalizedMainText> localizedTexts = _homeViewMono.transform.GetComponentsInChildren<LocalizedMainText>(true).ToList();
      localizedTexts.ForEach(item =>
      {
        item.Construct(localeSystem);
        item.Localize();
      });
    }

    private void CreateAnimations()
    {
      new ButtonAnimation(_homeViewMono.ShopButton.transform);
      new ButtonAnimation(_homeViewMono.CreateButton.transform);
      new ButtonAnimation(_homeViewMono.ConnectButton.transform);
      new ButtonAnimation(_homeViewMono.SettingsButton.transform);
      new ButtonAnimation(_homeViewMono.NoAdsButton.transform);
      new ButtonAnimation(_homeViewMono.BackButton.transform);
      
      new ButtonAnimation(_homeViewMono.ClassicButton.transform);
      new ButtonAnimation(_homeViewMono.BuhurtButton.transform);
    }

    private void CreateSounds()
    {
      new ButtonSound(_audioService, _homeViewMono.ShopButton.gameObject, AudioId.Click);
      new ButtonSound(_audioService, _homeViewMono.NoAdsButton.gameObject, AudioId.Click);
      new ButtonSound(_audioService, _homeViewMono.SettingsButton.gameObject, AudioId.Click);
      
      new ButtonSound(_audioService, _homeViewMono.EnemyTypeButton.gameObject, AudioId.Click);
      
      new ButtonSound(_audioService, _homeViewMono.ClassicButton.gameObject, AudioId.Click);
      new ButtonSound(_audioService, _homeViewMono.BuhurtButton.gameObject, AudioId.Click);
      
      new ButtonSound(_audioService, _homeViewMono.BackButton.gameObject, AudioId.ClickBack);
      new ButtonSound(_audioService, _homeViewMono.CreateButton.gameObject, AudioId.StartButton);
      
      _audioService.Play(_gameMetaProvider.GetMusic());
    }

    private void OnClickGame(Game gameMode)
    {
      SetEnemyTypePanel(true);
      _gameMetaProvider.SetGame(gameMode);

      Debug.Log("On game change " + _gameMetaProvider.GetGame());
    }

    private void SetEnemyTypePanel(bool isOn)
    {
      _homeViewMono.EnemyTypeCanvasGroup.blocksRaycasts = isOn;
      _homeViewMono.EnemyTypeCanvasGroup.DOFade(isOn ? 1f: 0f, SwitcherTime);
    }

    private void SwitchEnemy(bool isOneDevice/*EnemyType type*/)
    {
      _homeViewMono.SwitcherEnemyType.DOLocalMove(isOneDevice ? Vector3.left * 50 : Vector3.right * 50, SwitcherTime);

      _gameMetaProvider.SetEnemyType(isOneDevice ? EnemyType.OneDevice : EnemyType.Bot);
      Debug.Log("On enemy change " + _gameMetaProvider.GetEnemyType());
      // IconMusic.sprite = mute ? _musicOff : _musicOn;
      // var music = SwitcherMusic.parent.GetComponent<Image>();
      // music.sprite = mute ? Inactive : Active;
      // music.color = mute ? InactiveColor : Color.white;
    }
    
    public string GetInputIp() =>
      _homeViewMono.IpInputField.text;
  }
}