using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Animation;
using _Project.Scripts.Features.GameScreen.Mono;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Localization.Mono;
using _Project.Scripts.Services.Network;

namespace _Project.Scripts.Features.GameScreen
{
  public class GameViewLogic : IGameViewLogic
  {
    private readonly GameViewMono _gameViewMono;

    private readonly INetwork _network;
    private readonly ILocaleSystem _localeSystem;

    public GameViewLogic(GameViewMono gameViewMono, ILocaleSystem localeSystem, INetwork network)
    {
      _network = network;
      _localeSystem = localeSystem;
      _gameViewMono = gameViewMono;
    }

    public void Construct(IAudioService audioService, IGameFactory gameFactory, Action onBack, Action onLoadComplete)
    {
      // Localize(_localeSystem);
      _gameViewMono.gameObject.SetActive(false);
      // AppendClickSound(audioService);
      
      _gameViewMono.BackButton.onClick.AddListener(() => onBack?.Invoke());

      AppendButtonsAnimation();

      _gameViewMono.Construct(_network, gameFactory, onLoadComplete);
    }

    public void OpenWindow() =>
      AppearAnimation();

    private void AppearAnimation()
    {
      _gameViewMono.gameObject.SetActive(true);
    }

    private void Localize(ILocaleSystem localeSystem)
    {
      List<LocalizedMainText> localizedTexts = _gameViewMono.GetComponentsInChildren<LocalizedMainText>(true).ToList();
      localizedTexts.ForEach(item =>
      {
        item.Construct(localeSystem);
        item.Localize();
      });
    }

    private void AppendButtonsAnimation()
    {
      new ButtonAnimation(_gameViewMono.BackButton.transform);
    }

    private void AppendClickSound(IAudioService audioService)
    {
      _gameViewMono.BackButton.onClick.AddListener((() => audioService.Play(AudioId.SoundButton)));
    }
  }
}