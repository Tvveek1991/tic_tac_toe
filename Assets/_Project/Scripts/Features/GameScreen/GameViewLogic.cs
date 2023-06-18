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
    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;

    protected readonly GameViewMono GameViewMono;

    protected readonly IInputResolver InputResolver;
    protected readonly IGameMetaProvider GameMetaProvider;

    protected Action<SymbolSign> OnEndGame;
    protected Action OnCreateNonConnectionPlayer;

    private readonly Dictionary<SymbolSign, SymbolSize[]> _figuresState;

    public IDragDrop DragDrop { get; private set; }

    protected GameViewLogic(GameViewMono gameViewMono, ILocaleSystem localeSystem, IGameMetaProvider gameMetaProvider, IAudioService audioService,
      IInputResolver inputResolver)
    {
      GameViewMono = gameViewMono;
      InputResolver = inputResolver;
      GameMetaProvider = gameMetaProvider;

      _localeSystem = localeSystem;
      _audioService = audioService;

      _figuresState = new Dictionary<SymbolSign, SymbolSize[]>
      {
        {SymbolSign.X, new[] {SymbolSize.Tiny, SymbolSize.Small, SymbolSize.Medium, SymbolSize.Big, SymbolSize.Large}},
        {SymbolSign.O, new[] {SymbolSize.Tiny, SymbolSize.Small, SymbolSize.Medium, SymbolSize.Big, SymbolSize.Large}}
      };
    }

    public virtual void Construct(Action onBack, Action onCreateNonConnectionPlayer, Action<SymbolSign> onEndGame)
    {
      OnEndGame = onEndGame;
      OnCreateNonConnectionPlayer = onCreateNonConnectionPlayer;

      Localize(_localeSystem);
      GameViewMono.gameObject.SetActive(false);
      AppendClickSound(_audioService);

      GameViewMono.BackButton.onClick.AddListener(() => onBack?.Invoke());

      AppendButtonsAnimation();

      if (GameMetaProvider.GetGame() != Game.Buhurt)
        return;

      CreateDragDrop();
    }

    public void OpenWindow() =>
      AppearAnimation();

    public virtual void SetPlayerUi(SymbolSign symbol)
    {
      if(GameViewMono.SymbolImage == null)
        return;
      
      GameViewMono.SymbolImage.sprite = GameMetaProvider.GetIcon(symbol);
      GameViewMono.SymbolImage.color = symbol == SymbolSign.X ? Color.blue : Color.red;
    }

    public void SetFigureState(SymbolSign symbol, SymbolSize size) =>
      _figuresState[symbol][(int) size] = SymbolSize.Default;

    public virtual void ChangeTurningPlayer(SymbolSign activeSign) { }
    
    protected bool IsFiguresOver(SymbolSign symbolSign) => 
      _figuresState[symbolSign].All(item => item == SymbolSize.Default);
    
    protected void LoadBuhurtPart(SymbolSign symbol)
    {
      for (var i = 0; i < _figuresState[symbol].Length; i++)
        GameViewMono.GetFigures()[i].LoadFigure(symbol, _figuresState[symbol][i] == SymbolSize.Default,
          GameMetaProvider.GetMesh(symbol, (SymbolSize)i), GameMetaProvider.GetMaterial(symbol, (SymbolSize)i));
    }

    private void CreateDragDrop()
    {
      DragDrop = GameViewMono.gameObject.AddComponent<DragDrop>();
      GameViewMono.GetFigures().ForEach(item => item.Construct(DragDrop, InputResolver));
    }

    private void AppearAnimation()
    {
      GameViewMono.gameObject.SetActive(true);
    }

    private void Localize(ILocaleSystem localeSystem)
    {
      List<LocalizedMainText> localizedTexts = GameViewMono.GetComponentsInChildren<LocalizedMainText>(true).ToList();
      localizedTexts.ForEach(item =>
      {
        item.Construct(localeSystem);
        item.Localize();
      });
    }

    private void AppendButtonsAnimation() => 
      new ButtonAnimation(GameViewMono.BackButton.transform);

    private void AppendClickSound(IAudioService audioService) => 
      GameViewMono.BackButton.onClick.AddListener((() => audioService.Play(AudioId.ClickBack)));
  }
}