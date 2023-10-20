using System;
using _Project.Scripts.Animation;
using _Project.Scripts.Features.EndGameScreen.Mono;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio.Enum;
using _Project.Scripts.Services.Audio.Executor;
using _Project.Scripts.Services.Localization.Logic;
using DG.Tweening;

namespace _Project.Scripts.Features.EndGameScreen
{
  public class EndGameLogic : IEndGameLogic
  {
    private const float Duration = .25f;
    private readonly EndGameMono _endGameWindow;

    private readonly IAudioService _audioService;
    private readonly ILocaleSystem _localeSystem;
    private readonly IGameMetaProvider _gameMetaProvider;

    public EndGameLogic(EndGameMono endGameWindow, IAudioService audioService, ILocaleSystem localeSystem, IGameMetaProvider gameMetaProvider)
    {
      _audioService = audioService;
      _localeSystem = localeSystem;
      _gameMetaProvider = gameMetaProvider;

      _endGameWindow = endGameWindow;
    }

    public void Construct(Action onClickHome, Action onClickPlayAgain)
    {
      _endGameWindow.ButtonHome.onClick.AddListener(() => onClickHome?.Invoke());
      _endGameWindow.ButtonPlayAgain.onClick.AddListener(() => onClickPlayAgain?.Invoke());

      AddAnimations();
      AddSounds();
    }

    public void Show(SymbolSign sign)
    {
      _gameMetaProvider.AddScore(sign);

      LoadTitle(sign);
      LoadWinStreak(sign);
      LoadResult(sign);

      _endGameWindow.CanvasGroup.DOFade(1f, Duration)
        .OnComplete(() => { _endGameWindow.CanvasGroup.blocksRaycasts = true; });
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

    private void LoadWinStreak(SymbolSign sign)
    {
      if (_gameMetaProvider.GetEnemyType() != EnemyType.Bot)
        return;

      if (sign != SymbolSign.None)
        _gameMetaProvider.AddWinStreak(_gameMetaProvider.GetPlayerView().GetSymbol() == sign);

      _endGameWindow.WinStreakPanel.SetActive(true);

      if (_gameMetaProvider.GetWinStreak(true) < 2)
      {
        _endGameWindow.WinStreakText.gameObject.SetActive(false);
        _endGameWindow.BestWinStreakText.transform.parent.gameObject.SetActive(false);
        return;
      }

      _endGameWindow.WinStreakText.text = _localeSystem.GetText("EndGame.WinStreak") + " " + _gameMetaProvider.GetWinStreak(false);
      _endGameWindow.BestWinStreakText.text = _localeSystem.GetText("EndGame.BestWinStreak") + " " + _gameMetaProvider.GetWinStreak(true);
    }

    private void LoadResult(SymbolSign winnerSign)
    {
      if (_gameMetaProvider.GetEnemyType() != EnemyType.OneDevice)
        return;

      _endGameWindow.WinnerSymbol.sprite = winnerSign == SymbolSign.None ? _endGameWindow.DrawIcon : _gameMetaProvider.GetIcon(winnerSign);
      _endGameWindow.ScorePanel.SetActive(true);

      for (var i = 0; i < Enum.GetValues(typeof(SymbolSign)).Length - 1; i++)
      {
        var sign = (SymbolSign) (i + 1);
        _endGameWindow.ResultList.Find(item => item.SymbolSign == sign).Icon.sprite = _gameMetaProvider.GetIcon(sign);
        _endGameWindow.ResultList.Find(item => item.SymbolSign == sign).Score.text = _gameMetaProvider.GetScore(sign).ToString();
      }
    }

    private void LoadTitle(SymbolSign sign)
    {
      _endGameWindow.ResultIcon.sprite = 
        sign == SymbolSign.None 
          ? _endGameWindow.DrawIcon 
          : (sign == _gameMetaProvider.GetPlayerView().GetSymbol() ? _endGameWindow.WinIcon : _endGameWindow.LoseIcon);
      
      _endGameWindow.ResultText.text =
        sign == SymbolSign.None
        ? _localeSystem.GetText("EndGame.Draw") 
        : (sign == _gameMetaProvider.GetPlayerView().GetSymbol() ? _localeSystem.GetText("EndGame.Win")  : _localeSystem.GetText("EndGame.Lose"));
    }
  }
}