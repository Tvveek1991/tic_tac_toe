using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Animation;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.DragAndDrop;
using _Project.Scripts.Features.DragAndDrop.Interface;
using _Project.Scripts.Features.Field.Mono;
using _Project.Scripts.Features.GameScreen.Mono;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Features.Player.BuhurtModules;
using _Project.Scripts.Features.Tutorial;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio.Enum;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Localization.Mono;
using DG.Tweening;
using Sirenix.Utilities;

namespace _Project.Scripts.Features.GameScreen
{
  public class GameViewLogic : IGameViewLogic
  {
    private const float Duration = .25f;
    protected readonly IInputResolver InputResolver;
    protected readonly IGameMetaProvider GameMetaProvider;

    protected Action<SymbolSign> OnEndGame;

    protected readonly Dictionary<SymbolSign, SymbolSize[]> FiguresState = new()
    {
      {SymbolSign.X, new[] {SymbolSize.Tiny, SymbolSize.Small, SymbolSize.Medium, SymbolSize.Big, SymbolSize.Large}},
      {SymbolSign.O, new[] {SymbolSize.Tiny, SymbolSize.Small, SymbolSize.Medium, SymbolSize.Big, SymbolSize.Large}}
    };

    private readonly IAssetProvider _assetProvider;
    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;
    private readonly IGameFactory _gameFactory;

    private readonly GameViewMono _gameViewMono;

    private TutorialLogic _tutorialLogic;

    public IDragDrop DragDrop { get; private set; }

    protected GameViewLogic(GameViewMono gameViewMono, IGameFactory gameFactory, ILocaleSystem localeSystem, IGameMetaProvider gameMetaProvider,
      IAudioService audioService, IInputResolver inputResolver, IAssetProvider assetProvider)
    {
      _gameViewMono = gameViewMono;

      _localeSystem = localeSystem;
      _assetProvider = assetProvider;
      _audioService = audioService;
      _gameFactory = gameFactory;

      InputResolver = inputResolver;
      GameMetaProvider = gameMetaProvider;

      gameFactory.SpawnPlayerObject();

      if (GameMetaProvider.GetGame() != Game.Buhurt)
        return;

      CreateDragDrop();
    }

    public virtual void Construct(Action onBack, Action<SymbolSign> onEndGame)
    {
      OnEndGame = onEndGame;

      _tutorialLogic = new TutorialLogic(_assetProvider, _localeSystem);
      _tutorialLogic.Construct();

      _gameViewMono.BackButton.onClick.AddListener(() => onBack?.Invoke());
      _gameViewMono.TutorialButton.onClick.AddListener(() => _tutorialLogic.SetActivity(true));

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

    public void SetFigureState(SymbolSign symbol, SymbolSize size)
    {
      FiguresState[symbol][(int) size] = SymbolSize.Default;
      _gameViewMono.GetFigures().Find(item => item.Sign == symbol && item.Size == size)?.SetSizeToMoved();
      _gameViewMono.GetFigures(true).Find(item => item.Sign == symbol && item.Size == size)?.SetSizeToMoved();
    }

    public void HidePlayersPanels()
    {
      _gameViewMono.GetFigures().ForEach(item => item.SetVisibility(false));
      _gameViewMono.PlayerCanvasGroup.DOFade(0f, Duration);

      _gameViewMono.GetFigures(true).ForEach(item => item.SetVisibility(false));
      _gameViewMono.OpponentCanvasGroup.DOFade(0f, Duration);
    }

    public void Clear() =>
      _tutorialLogic.Exit();

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

      if (isPlayerTurn)
        ShowPotentialTurn(nextSign);
      else
        HidePotentialTurn();
    }

    private void SetPlayerUi(SymbolSign symbol)
    {
      bool isMainPlayer = GameMetaProvider.GetEnemyType() == EnemyType.OneDevice ? symbol == SymbolSign.X : symbol == GameMetaProvider.GetPlayerView().GetSymbol();

      _gameViewMono.GetSymbolPart().SetActivity(isMainPlayer);
      _gameViewMono.GetSymbolPart(true).SetActivity(!isMainPlayer);

      if (GameMetaProvider.GetGame() != Game.Buhurt)
        return;

      SetVisibilityFiguresPanel(isMainPlayer);
    }

    private void CreateDragDrop()
    {
      DragDrop = _gameViewMono.gameObject.AddComponent<DragDrop>();
      _gameViewMono.GetFigures().ForEach(item => item.Construct(DragDrop, InputResolver, ShowPotentialTurn, HidePotentialTurn));
      _gameViewMono.GetFigures(true).ForEach(item => item.Construct(DragDrop, InputResolver, ShowPotentialTurn, HidePotentialTurn));
    }

    private void LoadPlayerBuhurtPart(SymbolSign symbol)
    {
      for (var i = 0; i < FiguresState[symbol].Length; i++)
      {
        var dragableData = new DragableData(symbol, (SymbolSize) i);
        _gameViewMono.GetFigures()[i].FirstLoadFigure(symbol, GameMetaProvider.GetMesh(dragableData), GameMetaProvider.GetMaterial(dragableData));
      }
    }

    private void LoadOpponentBuhurtPart(SymbolSign symbol)
    {
      for (var i = 0; i < FiguresState[symbol].Length; i++)
      {
        var dragableData = new DragableData(symbol, (SymbolSize) i, GameMetaProvider.GetEnemyType() == EnemyType.Bot);
        _gameViewMono.GetFigures(true)[i].FirstLoadFigure(symbol, GameMetaProvider.GetMesh(dragableData), GameMetaProvider.GetMaterial(dragableData));
      }
    }
    
    private const int FieldSizeX = 3;
    private const int FieldSizeY = 3;
    private void ShowPotentialTurn(SymbolSign symbolSign)
    {
      if(GameMetaProvider.GetGame() == Game.Buhurt)
        return;
      
      var symbolCells = new SymbolCell[_gameFactory.GameFieldLogic.GetField().Length];
      var board = new SymbolSign[_gameFactory.GameFieldLogic.GetField().Length];
      
      for (var y = 0; y < FieldSizeY; y++)
      for (var x = 0; x < FieldSizeX; x++)
      {
        symbolCells[3 * y + x] = _gameFactory.GameFieldLogic.GetField()[y, x];
        board[3 * y + x] = _gameFactory.GameFieldLogic.GetField()[y, x].GetSymbol();
      }

      symbolCells.ForEach(item =>
      {
        if (item.GetSymbol() == SymbolSign.None)
        {
          var dragableData = new DragableData(symbolSign, SymbolSize.Default);
          item.ShowPotentialTurn(GameMetaProvider.GetMesh(dragableData));
        }
      });
    }
    private void ShowPotentialTurn(DragablePart dragablePart)
    {
      var symbolCells = new SymbolCell[_gameFactory.GameFieldLogic.GetField().Length];
      var board = new SymbolSign[_gameFactory.GameFieldLogic.GetField().Length];
      
      for (var y = 0; y < FieldSizeY; y++)
      for (var x = 0; x < FieldSizeX; x++)
      {
        symbolCells[3 * y + x] = _gameFactory.GameFieldLogic.GetField()[y, x];
        board[3 * y + x] = _gameFactory.GameFieldLogic.GetField()[y, x].GetSymbol();
      }

      symbolCells.ForEach(item =>
      {
        if (item.GetSize() < dragablePart.GetDragableData().GetSize() && item.GetSymbol() != dragablePart.GetDragableData().GetSymbol())
        {
          var dragableData = new DragableData(dragablePart.GetDragableData().GetSymbol(), dragablePart.GetDragableData().GetSize());
          item.ShowPotentialTurn(GameMetaProvider.GetMesh(dragableData));
        }
      });
    }
    
    private void HidePotentialTurn()
    {
      foreach (SymbolCell symbolCell in _gameFactory.GameFieldLogic.GetField())
        symbolCell.HidePotentialTurn();
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