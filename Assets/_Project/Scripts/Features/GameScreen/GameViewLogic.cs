using System;
using System.Collections.Generic;
using _Project.Scripts.Animation;
using _Project.Scripts.Features.DragAndDrop;
using _Project.Scripts.Features.DragAndDrop.Interface;
using _Project.Scripts.Features.GameScreen.Mono;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.GameMetaProvider;
using _Project.Scripts.Services.Localization.Logic;

namespace _Project.Scripts.Features.GameScreen
{
  public class GameViewLogic : IGameViewLogic
  {
    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;

    protected Dictionary<SymbolSign, PlayerData> PlayerData;

    protected readonly GameViewMono GameViewMono;

    protected readonly IInputResolver InputResolver;
    protected readonly IGameMetaProvider GameMetaProvider;

    protected Action OnCreateNonConnectionPlayer;

    protected readonly Dictionary<SymbolSign, SymbolSize[]> FiguresState;

    public IDragDrop DragDrop { get; private set; }

    protected GameViewLogic(GameViewMono gameViewMono, ILocaleSystem localeSystem, IGameMetaProvider gameMetaProvider, IAudioService audioService, IInputResolver inputResolver)
    {
      GameViewMono = gameViewMono;
      InputResolver = inputResolver;
      GameMetaProvider = gameMetaProvider;

      _localeSystem = localeSystem;
      _audioService = audioService;

      FiguresState = new Dictionary<SymbolSign, SymbolSize[]>
      {
        {SymbolSign.X, new[] {SymbolSize.Tiny, SymbolSize.Small, SymbolSize.Medium, SymbolSize.Big, SymbolSize.Large}},
        {SymbolSign.O, new[] {SymbolSize.Tiny, SymbolSize.Small, SymbolSize.Medium, SymbolSize.Big, SymbolSize.Large}}
      };
    }

    public virtual void Construct(Action onBack, Action onCreateNonConnectionPlayer)
    {
      OnCreateNonConnectionPlayer = onCreateNonConnectionPlayer;
      PlayerData = GameMetaProvider.GetPlayerData();

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
      GameViewMono.SymbolImage.sprite = PlayerData[symbol].SymbolData.Icon;
      GameViewMono.SymbolImage.color = PlayerData[symbol].Color;
    }

    public void SetFigureState(SymbolSign symbol, SymbolSize size) => 
      FiguresState[symbol][(int) size] = SymbolSize.Default;

    public virtual void ChangeTurningPlayer(SymbolSign activeSign) { }

    private void CreateDragDrop()
    {
      DragDrop = GameViewMono.gameObject.AddComponent<DragDrop>();
      GameViewMono.GetFigures().ForEach(item => item.Construct(DragDrop));
    }

    private void AppearAnimation()
    {
      GameViewMono.gameObject.SetActive(true);
    }

    private void Localize(ILocaleSystem localeSystem)
    {
      /*List<LocalizedMainText> localizedTexts = GameViewMono.GetComponentsInChildren<LocalizedMainText>(true).ToList();
      localizedTexts.ForEach(item =>
      {
        item.Construct(localeSystem);
        item.Localize();
      });*/
    }

    private void AppendButtonsAnimation()
    {
      new ButtonAnimation(GameViewMono.BackButton.transform);
    }

    private void AppendClickSound(IAudioService audioService)
    {
      // GameViewMono.BackButton.onClick.AddListener((() => audioService.Play(AudioId.SoundButton)));
    }
  }
}