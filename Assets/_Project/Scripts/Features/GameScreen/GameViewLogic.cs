using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Animation;
using _Project.Scripts.Features.GameScreen.Mono;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Localization.Mono;

namespace _Project.Scripts.Features.GameScreen
{
  public class GameViewLogic : IGameViewLogic
  {
    private readonly GameView _gameView;

    private readonly ILocaleSystem _localeSystem;

    public GameViewLogic(GameView gameView, ILocaleSystem localeSystem)
    {
      _gameView = gameView;
      _localeSystem = localeSystem;
    }

    public void Construct(IAudioService audioService, Action onBack)
    {
      // Localize(_localeSystem);
      _gameView.gameObject.SetActive(false);
      // AppendClickSound(audioService);

      _gameView.BackButton.onClick.AddListener(() => onBack?.Invoke());
      
      AppendButtonsAnimation();
    }

    public void OpenWindow() =>
      AppearAnimation();

    private void AppearAnimation()
    {
      _gameView.gameObject.SetActive(true);
    }
    
    private void Localize(ILocaleSystem localeSystem)
    {
      List<LocalizedMainText> localizedTexts = _gameView.GetComponentsInChildren<LocalizedMainText>(true).ToList();
      localizedTexts.ForEach(item =>
      {
        item.Construct(localeSystem);
        item.Localize();
      });
    }

    private void AppendButtonsAnimation()
    {
      new ButtonAnimation(_gameView.BackButton.transform);
    }

    private void AppendClickSound(IAudioService audioService)
    {
      _gameView.BackButton.onClick.AddListener((() => audioService.Play(AudioId.SoundButton)));
    }
  }
}