using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Animation;
using _Project.Scripts.Features.DragAndDrop;
using _Project.Scripts.Features.DragAndDrop.Interface;
using _Project.Scripts.Features.GameScreen.Mono;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio.Enum;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Localization.Mono;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Features.GameScreen
{
  public class GameViewLogic : IGameViewLogic
  {
    private const float Duration = .25f;
    protected readonly IInputResolver InputResolver;
    protected readonly IGameMetaProvider GameMetaProvider;

    protected Action<SymbolSign> OnEndGame;

    protected readonly Dictionary<SymbolSign, SymbolSize[]> FiguresState;

    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;

    private readonly GameViewMono _gameViewMono;

    public IDragDrop DragDrop { get; private set; }

    protected GameViewLogic(GameViewMono gameViewMono, IGameFactory gameFactory, ILocaleSystem localeSystem, IGameMetaProvider gameMetaProvider,
      IAudioService audioService, IInputResolver inputResolver)
    {
      _gameViewMono = gameViewMono;

      _localeSystem = localeSystem;
      _audioService = audioService;
      
      InputResolver = inputResolver;
      GameMetaProvider = gameMetaProvider;
      
      gameFactory.SpawnPlayerObject();
      
      if (GameMetaProvider.GetGame() != Game.Buhurt)
        return;

      FiguresState = new Dictionary<SymbolSign, SymbolSize[]>
      {
        {SymbolSign.X, new[] {SymbolSize.Tiny, SymbolSize.Small, SymbolSize.Medium, SymbolSize.Big, SymbolSize.Large}},
        {SymbolSign.O, new[] {SymbolSize.Tiny, SymbolSize.Small, SymbolSize.Medium, SymbolSize.Big, SymbolSize.Large}}
      };

      CreateDragDrop();
    }

    public virtual void Construct(Action onBack, Action<SymbolSign> onEndGame)
    {
      OnEndGame = onEndGame;

      _gameViewMono.BackButton.onClick.AddListener(() => onBack?.Invoke());
      
      Localize(_localeSystem);
      AppendButtonsAnimation();
      AppendClickSound(_audioService);

      FirstLoadPlayersUI(GameMetaProvider.GetGame());
      
      RefreshAfterChangeTurn(SymbolSign.X);
    }

    private void FirstLoadPlayersUI(Game game)
    {
      SymbolSign symbol = GameMetaProvider.GetPlayerView().GetSymbol();
      
      if (game == Game.Classic)
      {
        _gameViewMono.GetSymbolPart().Construct(GameMetaProvider.GetIcon(symbol));
        _gameViewMono.GetSymbolPart(true).Construct(GameMetaProvider.GetIcon(symbol == SymbolSign.X ? SymbolSign.O : SymbolSign.X));
      }
      else
      {
        LoadPlayerBuhurtPart(symbol);
        LoadOpponentBuhurtPart(symbol == SymbolSign.X ? SymbolSign.O : SymbolSign.X);
      }
    }

    public virtual void ChangeTurningPlayer(SymbolSign activeSign)
    {
      SymbolSign nextSign = activeSign == SymbolSign.X ? SymbolSign.O : SymbolSign.X;
      RefreshAfterChangeTurn(nextSign);
    }

    public void OpenWindow() =>
      AppearAnimation();

    public void SetFigureState(SymbolSign symbol, SymbolSize size) =>
      FiguresState[symbol][(int) size] = SymbolSize.Default;

    public void HidePlayersPanels()
    {
      _gameViewMono.GetFigures().ForEach(item => item.SetVisibility(false));
      _gameViewMono.PlayerCanvasGroup.DOFade(0f, Duration);
      
      _gameViewMono.GetFigures(true).ForEach(item => item.SetVisibility(false));
      _gameViewMono.OpponentCanvasGroup.DOFade(0f, Duration);
    }

    private void SetVisibilityFiguresPanel(bool isMainPlayer = false)
    {
      _gameViewMono.GetFigures().ForEach(item => item.SetVisibility(isMainPlayer));
      _gameViewMono.PlayerCanvasGroup.DOFade(isMainPlayer ? 1f : .05f, Duration);
      
      _gameViewMono.GetFigures(true).ForEach(item => item.SetVisibility(!isMainPlayer));
      _gameViewMono.OpponentCanvasGroup.DOFade(!isMainPlayer ? 1f : .05f, Duration);
    }

    protected void RefreshAfterChangeTurn(SymbolSign nextSign)
    {
      if (IsBuhurtFiguresOver())
        return;

      bool isPlayerTurn = GameMetaProvider.GetEnemyType() == EnemyType.OneDevice || nextSign == GameMetaProvider.GetPlayerView().GetSymbol();

      SetPlayerUi(nextSign);
      GameMetaProvider.GetPlayerView().SetPlayerTurn(isPlayerTurn);
    }

    private void SetPlayerUi(SymbolSign symbol)
    {
      bool isMainPlayer = GameMetaProvider.GetEnemyType() == EnemyType.OneDevice ? 
        symbol == SymbolSign.X : symbol == GameMetaProvider.GetPlayerView().GetSymbol();

      _gameViewMono.GetSymbolPart().SetActivity(isMainPlayer);
      _gameViewMono.GetSymbolPart(true).SetActivity(!isMainPlayer);

      if (GameMetaProvider.GetGame() != Game.Buhurt)
        return;

      SetVisibilityFiguresPanel(isMainPlayer);
    }

    private bool IsBuhurtFiguresOver()
    {
      if (GameMetaProvider.GetGame() != Game.Buhurt)
        return false;

      if (!CheckLastFiguresSymbol() && !IsFiguresOver())
        return false;
      
      OnEndGame?.Invoke(SymbolSign.None);
      return true;
    }

    private void CreateDragDrop()
    {
      DragDrop = _gameViewMono.gameObject.AddComponent<DragDrop>();
      _gameViewMono.GetFigures().ForEach(item => item.Construct(DragDrop, InputResolver));
      _gameViewMono.GetFigures(true).ForEach(item => item.Construct(DragDrop, InputResolver));
    }

    private void LoadPlayerBuhurtPart(SymbolSign symbol)
    {
      for (var i = 0; i < FiguresState[symbol].Length; i++)
        _gameViewMono.GetFigures()[i].LoadFigure(symbol, FiguresState[symbol][i] == SymbolSize.Default,
          GameMetaProvider.GetMesh(symbol, (SymbolSize) i), GameMetaProvider.GetMaterial(symbol, (SymbolSize) i));
    }
    
    private void LoadOpponentBuhurtPart(SymbolSign symbol)
    {
      for (var i = 0; i < FiguresState[symbol].Length; i++)
        _gameViewMono.GetFigures(true)[i].LoadFigure(symbol, FiguresState[symbol][i] == SymbolSize.Default,
          GameMetaProvider.GetMesh(symbol, (SymbolSize) i, GameMetaProvider.GetEnemyType() == EnemyType.Bot),
          GameMetaProvider.GetMaterial(symbol, (SymbolSize) i, GameMetaProvider.GetEnemyType() == EnemyType.Bot));
    }

    private void AppearAnimation() => 
      _gameViewMono.gameObject.SetActive(true);

    private void Localize(ILocaleSystem localeSystem)
    {
      List<LocalizedMainText> localizedTexts = _gameViewMono.GetComponentsInChildren<LocalizedMainText>(true).ToList();
      localizedTexts.ForEach(item =>
      {
        item.Construct(localeSystem);
        item.Localize();
      });
    }

    private void AppendButtonsAnimation() =>
      new ButtonAnimation(_gameViewMono.BackButton.transform);

    private void AppendClickSound(IAudioService audioService) =>
      _gameViewMono.BackButton.onClick.AddListener((() => audioService.Play(AudioId.ClickBack)));

    private bool IsFiguresOver() =>
      FiguresState[SymbolSign.X].All(item => item == SymbolSize.Default)
      && FiguresState[SymbolSign.O].All(item => item == SymbolSize.Default);

    private bool CheckLastFiguresSymbol() =>
      FiguresState[SymbolSign.X].Count(size => size == SymbolSize.Default) + FiguresState[SymbolSign.O].Count(size => size == SymbolSize.Default) == 9
      && (FiguresState[SymbolSign.X].Contains(SymbolSize.Tiny) || FiguresState[SymbolSign.O].Contains(SymbolSize.Tiny));
  }
}