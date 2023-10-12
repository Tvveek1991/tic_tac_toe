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
using UnityEngine.UI;

namespace _Project.Scripts.Features.HomeScreen
{
  public class HomeViewLogic : IHomeViewLogic
  {
    private const float Duration = .25f;

    private readonly HomeViewMono _homeViewMono;
    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;
    private readonly IGameMetaProvider _gameMetaProvider;

    private Action _toGame;
    
    public HomeViewLogic(HomeViewMono homeViewMono, ILocaleSystem localeSystem, IAudioService audioService, IGameMetaProvider gameMetaProvider)
    {
      _homeViewMono = homeViewMono;
      _localeSystem = localeSystem;
      _audioService = audioService;
      _gameMetaProvider = gameMetaProvider;
    }

    public void Construct(Action openSettingsWindow, Action toShop, Action toGame, Action toNoAds)
    {
      _toGame = toGame;
      
      _homeViewMono.ShopButton.onClick.AddListener(toShop.Invoke);
      _homeViewMono.NoAdsButton.onClick.AddListener(toNoAds.Invoke);
      _homeViewMono.SettingsButton.onClick.AddListener(openSettingsWindow.Invoke);

      _homeViewMono.ClassicButton.onClick.AddListener(() => OnClickGame(Game.Classic));
      _homeViewMono.BuhurtButton.onClick.AddListener(() => OnClickGame(Game.Buhurt));

      _homeViewMono.FriendsButton.onClick.AddListener(() => SwitchEnemy(true));
      _homeViewMono.SingleButton.onClick.AddListener(() => SwitchEnemy(false));

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
      // new ButtonAnimation(_homeViewMono.ShopButton.transform);
      new ButtonAnimation(_homeViewMono.SettingsButton.transform);
      new ButtonAnimation(_homeViewMono.NoAdsButton.transform);
      
      new ButtonAnimation(_homeViewMono.ClassicButton.transform);
      new ButtonAnimation(_homeViewMono.BuhurtButton.transform);
    }

    private void CreateSounds()
    {
      new ButtonSound(_audioService, _homeViewMono.ShopButton.gameObject, AudioId.Click);
      new ButtonSound(_audioService, _homeViewMono.NoAdsButton.gameObject, AudioId.Click);
      new ButtonSound(_audioService, _homeViewMono.SettingsButton.gameObject, AudioId.Click);
      
      new ButtonSound(_audioService, _homeViewMono.ClassicButton.gameObject, AudioId.Click);
      new ButtonSound(_audioService, _homeViewMono.BuhurtButton.gameObject, AudioId.Click);
      
      _audioService.Play(_gameMetaProvider.GetMusic());
    }

    private void OnClickGame(Game gameMode)
    {
      _gameMetaProvider.SetGame(gameMode);
      _toGame.Invoke();
      Debug.Log("On game change " + _gameMetaProvider.GetGame());
    }

    private void SwitchEnemy(bool isOneDevice)
    {
      _homeViewMono.SingleButton.GetComponent<Image>().DOFade(isOneDevice? .05f : 1f, Duration);
      _homeViewMono.FriendsButton.GetComponent<Image>().DOFade(isOneDevice? 1f : .05f, Duration);

      _gameMetaProvider.SetEnemyType(isOneDevice ? EnemyType.OneDevice : EnemyType.Bot);
      Debug.Log("On enemy change " + _gameMetaProvider.GetEnemyType());
    }
  }
}