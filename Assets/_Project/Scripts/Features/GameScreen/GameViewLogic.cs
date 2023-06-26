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
using UnityEngine;

namespace _Project.Scripts.Features.GameScreen
{
  public class GameViewLogic : IGameViewLogic
  {
    protected readonly IInputResolver InputResolver;
    protected readonly IGameMetaProvider GameMetaProvider;

    protected Action<SymbolSign> OnEndGame;

    protected readonly Dictionary<SymbolSign, SymbolSize[]> FiguresState;

    private readonly IGameFactory _gameFactory;
    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;

    private readonly GameViewMono _gameViewMono;

    private Action _onCreateNonConnectionPlayer;

    public IDragDrop DragDrop { get; private set; }

    protected GameViewLogic(GameViewMono gameViewMono, IGameFactory gameFactory, ILocaleSystem localeSystem, IGameMetaProvider gameMetaProvider,
      IAudioService audioService, IInputResolver inputResolver)
    {
      _gameViewMono = gameViewMono;

      _gameFactory = gameFactory;
      InputResolver = inputResolver;
      GameMetaProvider = gameMetaProvider;

      _localeSystem = localeSystem;
      _audioService = audioService;

      if (GameMetaProvider.GetGame() != Game.Buhurt)
        return;

      FiguresState = new Dictionary<SymbolSign, SymbolSize[]>
      {
        {SymbolSign.X, new[] {SymbolSize.Tiny, SymbolSize.Small, SymbolSize.Medium, SymbolSize.Big, SymbolSize.Large}},
        {SymbolSign.O, new[] {SymbolSize.Tiny, SymbolSize.Small, SymbolSize.Medium, SymbolSize.Big, SymbolSize.Large}}
      };

      CreateDragDrop();
    }

    public virtual void Construct(Action onBack, Action onCreateNonConnectionPlayer, Action<SymbolSign> onEndGame)
    {
      OnEndGame = onEndGame;
      _onCreateNonConnectionPlayer = onCreateNonConnectionPlayer;

      Localize(_localeSystem);
      AppendClickSound(_audioService);

      _gameViewMono.BackButton.onClick.AddListener(() => onBack?.Invoke());

      AppendButtonsAnimation();

      _onCreateNonConnectionPlayer?.Invoke();

      GameMetaProvider.GetPlayerView().Construct(_gameFactory, GameMetaProvider, InputResolver);

      _gameViewMono.HideLoadScreen();
      RefreshAfterChangeTurn(SymbolSign.X);

      if (GameMetaProvider.GetGame() != Game.Buhurt)
        return;

      _gameViewMono.SetVisibleBuhurtPart(true);
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

    public void HideBottomPanel() =>
      _gameViewMono.SetVisibleBuhurtPart(false);

    protected void RefreshAfterChangeTurn(SymbolSign nextSign)
    {
      if (IsBuhurtFiguresOver())
        return;

      bool isPlayerTurn = GameMetaProvider.GetEnemyType() == EnemyType.OneDevice || nextSign == GameMetaProvider.GetPlayerView().GetSymbol();

      GameMetaProvider.GetPlayerView().SetPlayerTurn(isPlayerTurn);
      SetPlayerUi(nextSign);
      _gameViewMono.TurnText.text = nextSign == SymbolSign.X ? "Player X" : "Player O";
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

    private void SetPlayerUi(SymbolSign symbol)
    {
      if (_gameViewMono.SymbolImage == null)
        return;

      _gameViewMono.SymbolImage.sprite = GameMetaProvider.GetIcon(symbol);
      _gameViewMono.SymbolImage.color = symbol == SymbolSign.X ? Color.blue : Color.red;

      if (GameMetaProvider.GetGame() != Game.Buhurt)
        return;

      LoadBuhurtPart(symbol);
    }

    private void CreateDragDrop()
    {
      DragDrop = _gameViewMono.gameObject.AddComponent<DragDrop>();
      _gameViewMono.GetFigures().ForEach(item => item.Construct(DragDrop, InputResolver));
    }

    private void LoadBuhurtPart(SymbolSign symbol)
    {
      for (var i = 0; i < FiguresState[symbol].Length; i++)
        _gameViewMono.GetFigures()[i].LoadFigure(symbol, FiguresState[symbol][i] == SymbolSize.Default,
          GameMetaProvider.GetMesh(symbol, (SymbolSize) i), GameMetaProvider.GetMaterial(symbol, (SymbolSize) i));
    }

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