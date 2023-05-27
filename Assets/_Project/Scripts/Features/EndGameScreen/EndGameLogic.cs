using System;
using _Project.Scripts.Animation;
using _Project.Scripts.Features.EndGameScreen.Mono;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio.Enum;
using _Project.Scripts.Services.Audio.Executor;

namespace _Project.Scripts.Features.EndGameScreen
{
  public class EndGameLogic : IEndGameLogic
  {
    private readonly EndGameMono _endGameWindow;
    
    private readonly IAudioService _audioService;
    private readonly IGameMetaProvider _gameMetaProvider;

    public EndGameLogic(EndGameMono endGameWindow, IAudioService audioService)
    {
      _endGameWindow = endGameWindow;
      _audioService = audioService;
    }

    public void Construct(Action onClickHome, Action onClickPlayAgain)
    {
      _endGameWindow.ButtonHome.onClick.AddListener(() => onClickHome?.Invoke());
      _endGameWindow.ButtonPlayAgain.onClick.AddListener(() => onClickPlayAgain?.Invoke());

      AddAnimations();
      AddSounds();
    }

    private void AddSounds()
    {
      new ButtonSound(_audioService, _endGameWindow.ButtonHome.gameObject, AudioId.ClickBack);
      new ButtonSound(_audioService, _endGameWindow.ButtonPlayAgain.gameObject, AudioId.Click);
    }

    private void AddAnimations()
    {
      new ButtonAnimation(_endGameWindow.ButtonHome.transform);
      new ButtonAnimation(_endGameWindow.ButtonPlayAgain.transform);
    }

    public void Show() => 
      _endGameWindow.gameObject.SetActive(true);
  }
}