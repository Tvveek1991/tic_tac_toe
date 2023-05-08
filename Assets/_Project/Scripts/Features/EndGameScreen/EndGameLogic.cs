using System;
using _Project.Scripts.Features.EndGameScreen.Mono;
using _Project.Scripts.Services._Interfaces;

namespace _Project.Scripts.Features.EndGameScreen
{
  public class EndGameLogic : IEndGameLogic
  {
    private readonly EndGameMono _endGameWindow;
    
    private readonly IAudioService _audioService;
    private readonly IGameMetaProvider _gameMetaProvider;

    public EndGameLogic(EndGameMono endGameWindow, IAudioService audioService, IGameMetaProvider gameMetaProvider)
    {
      _endGameWindow = endGameWindow;
      
      _audioService = audioService;
      gameMetaProvider = gameMetaProvider;
    }

    public void Construct(Action onClickHome, Action onClickPlayAgain)
    {
      _endGameWindow.ButtonHome.onClick.AddListener(() => onClickHome?.Invoke());
      _endGameWindow.ButtonPlayAgain.onClick.AddListener(() => onClickPlayAgain?.Invoke());
    }

    public void Show()
    {
      _endGameWindow.gameObject.SetActive(true);
    }
  }
}