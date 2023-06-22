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

    private readonly IGameFactory _gameFactory;
    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;

    private readonly GameViewMono _gameViewMono;

    private Action _onCreateNonConnectionPlayer;

    private readonly Dictionary<SymbolSign, SymbolSize[]> _figuresState;

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

      _figuresState = new Dictionary<SymbolSign, SymbolSize[]>
      {
        {SymbolSign.X, new[] {SymbolSize.Tiny, SymbolSize.Small, SymbolSize.Medium, SymbolSize.Big, SymbolSize.Large}},
        {SymbolSign.O, new[] {SymbolSize.Tiny, SymbolSize.Small, SymbolSize.Medium, SymbolSize.Big, SymbolSize.Large}}
      };
      
      if (GameMetaProvider.GetGame() != Game.Buhurt)
        return;

      CreateDragDrop();
    }

    public virtual void Construct(Action onBack, Action onCreateNonConnectionPlayer, Action<SymbolSign> onEndGame)
    {
      OnEndGame = onEndGame;
      _onCreateNonConnectionPlayer = onCreateNonConnectionPlayer;

      Localize(_localeSystem);
      _gameViewMono.gameObject.SetActive(false);
      AppendClickSound(_audioService);

      _gameViewMono.BackButton.onClick.AddListener(() => onBack?.Invoke());

      AppendButtonsAnimation();

      _onCreateNonConnectionPlayer?.Invoke();

      GameMetaProvider.GetPlayerView().Construct(_gameFactory, GameMetaProvider, InputResolver);

      _gameViewMono.HideLoadScreen();
      RefreshAfterChangeTurn(SymbolSign.X);
    }

    public virtual void ChangeTurningPlayer(SymbolSign activeSign)
    {
      SymbolSign nextSign = activeSign == SymbolSign.X ? SymbolSign.O : SymbolSign.X;
      RefreshAfterChangeTurn(nextSign);
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
      _gameViewMono.SetVisibleBuhurtPart(true);
    }

    public void OpenWindow() =>
      AppearAnimation();

    public void SetFigureState(SymbolSign symbol, SymbolSize size) =>
      _figuresState[symbol][(int) size] = SymbolSize.Default;

    protected bool IsFiguresOver(SymbolSign symbolSign) =>
      _figuresState[symbolSign].All(item => item == SymbolSize.Default);

    protected virtual void RefreshAfterChangeTurn(SymbolSign nextSign)
    {
      bool isPlayerTurn = GameMetaProvider.GetEnemyType() == EnemyType.OneDevice || nextSign == GameMetaProvider.GetPlayerView().GetSymbol();

      GameMetaProvider.GetPlayerView().SetPlayerTurn(isPlayerTurn);
      SetPlayerUi(nextSign);
      _gameViewMono.TurnText.text = nextSign == SymbolSign.X ? "Player X" : "Player O";
    }

    private void CreateDragDrop()
    {
      DragDrop = _gameViewMono.gameObject.AddComponent<DragDrop>();
      _gameViewMono.GetFigures().ForEach(item => item.Construct(DragDrop, InputResolver));
    }

    private void LoadBuhurtPart(SymbolSign symbol)
    {
      for (var i = 0; i < _figuresState[symbol].Length; i++)
        _gameViewMono.GetFigures()[i].LoadFigure(symbol, _figuresState[symbol][i] == SymbolSize.Default,
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
  }
}