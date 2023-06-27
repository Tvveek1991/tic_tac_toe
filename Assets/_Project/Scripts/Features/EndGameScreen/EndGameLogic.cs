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
    private const float Duration = .5f;
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

    public void Show(SymbolSign sign)
    {
      _gameMetaProvider.AddScore(sign);

      LoadTitle(sign);
      LoadWinStreak(sign);
      LoadResult();

      _endGameWindow.CanvasGroup.DOFade(1f, Duration)
        .OnComplete(() => { _endGameWindow.CanvasGroup.blocksRaycasts = true; });
    }

    private void LoadWinStreak(SymbolSign sign)
    {
      if (_gameMetaProvider.GetEnemyType() != EnemyType.Bot)
        return;

      if (sign != SymbolSign.None)
        _gameMetaProvider.AddWinStreak(_gameMetaProvider.GetPlayerView().GetSymbol() == sign);

      if (_gameMetaProvider.GetWinStreak(true) < 2)
        return;

      _endGameWindow.BestWinStreakText.transform.parent.gameObject.SetActive(true);
      _endGameWindow.WinStreakText.text = _localeSystem.GetText("EndGame.WinStreak") + " " + _gameMetaProvider.GetWinStreak(false);
      _endGameWindow.BestWinStreakText.text = _localeSystem.GetText("EndGame.BestWinStreak") + " " + _gameMetaProvider.GetWinStreak(true);
    }

    private void LoadResult()
    {
      if (_gameMetaProvider.GetEnemyType() != EnemyType.OneDevice)
        return;

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
      if (sign == SymbolSign.None)
      {
        _endGameWindow.TitleIcon.gameObject.SetActive(false);
        _endGameWindow.TitleText.text = _localeSystem.GetText("EndGame.Draw");
      }
      else
      {
        _endGameWindow.TitleIcon.sprite = _gameMetaProvider.GetIcon(sign);
        _endGameWindow.TitleText.text = _localeSystem.GetText("EndGame.Winner");
      }
    }
  }
}