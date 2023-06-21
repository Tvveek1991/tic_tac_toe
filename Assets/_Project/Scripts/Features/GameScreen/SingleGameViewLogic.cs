using System;
using _Project.Scripts.Features.GameScreen.Mono;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Features.Player.Bots;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;

namespace _Project.Scripts.Features.GameScreen
{
  public class SingleGameViewLogic : GameViewLogic
  {
    private readonly Bot _bot;

    private readonly IGameFactory _gameFactory;

    public SingleGameViewLogic(GameViewMono gameViewMono, IGameFactory gameFactory, ILocaleSystem localeSystem, IGameMetaProvider gameMetaProvider,
      IAudioService audioService, IInputResolver inputResolver)
      : base(gameViewMono, localeSystem, gameMetaProvider, audioService, inputResolver)
    {
      _gameFactory = gameFactory;
      _bot = GameMetaProvider.GetGame() == Game.Classic ? new ClassicBot(_gameFactory, gameMetaProvider) : new BuhurtBot(_gameFactory, gameMetaProvider);
    }

    public override void Construct(Action onBack, Action onCreateNonConnectionPlayer, Action<SymbolSign> onEndGame)
    {
      base.Construct(onBack, onCreateNonConnectionPlayer, onEndGame);

      OnCreateNonConnectionPlayer?.Invoke();

      GameMetaProvider.GetPlayerView().Construct(_gameFactory, GameMetaProvider, InputResolver);
      GameMetaProvider.GetPlayerView().CreateSymbolSign();
      _bot.SetBotSign();

      GameViewMono.HideLoadScreen();
      RefreshAfterChangeTurn(SymbolSign.X);
      
      if(GameMetaProvider.GetEnemyType() == EnemyType.Bot && GameMetaProvider.GetPlayerView().GetSymbol() == SymbolSign.O)
        BotTurn();
    }

    public override void ChangeTurningPlayer(SymbolSign activeSign)
    {
      SymbolSign nextSign = activeSign == SymbolSign.X ? SymbolSign.O : SymbolSign.X;
      RefreshAfterChangeTurn(nextSign);

      if (GameMetaProvider.GetEnemyType() == EnemyType.Bot && !InputResolver.IsLocked)
        BotTurn();
      else if (GameMetaProvider.GetEnemyType() == EnemyType.OneDevice)
        GameMetaProvider.GetPlayerView().ExchangeSymbolSign();
    }

    public override void SetPlayerUi(SymbolSign symbol)
    {
      base.SetPlayerUi(symbol);

      if (GameMetaProvider.GetGame() != Game.Buhurt)
        return;

      LoadBuhurtPart(symbol);
      GameViewMono.SetVisibleBuhurtPart(true);
    }

    private void RefreshAfterChangeTurn(SymbolSign nextSign)
    {
      if(IsBuhurtFiguresOver())
        return;

      bool isPlayerTurn = GameMetaProvider.GetEnemyType() == EnemyType.OneDevice || nextSign == GameMetaProvider.GetPlayerView().GetSymbol();

      GameMetaProvider.GetPlayerView().SetPlayerTurn(isPlayerTurn);
      SetPlayerUi(nextSign);
      GameViewMono.TurnText.text = nextSign == SymbolSign.X ? "Player X" : "Player O";
    }

    private bool IsBuhurtFiguresOver()
    {
      if (GameMetaProvider.GetGame() != Game.Buhurt)
        return false;
      
      if (_bot.IsFiguresOver() && IsFiguresOver(SymbolSign.X) || IsFiguresOver(SymbolSign.X) && IsFiguresOver(SymbolSign.O))
      {
        OnEndGame?.Invoke(SymbolSign.None);
        return true;
      }

      return false;
    }

    private void BotTurn()
    {
      InputResolver.Disable();
      _bot?.BotTurn(OnTurnEnd, () => OnEndGame?.Invoke(SymbolSign.None));

      void OnTurnEnd()
      {
        InputResolver.Enable();
        RefreshAfterChangeTurn(GameMetaProvider.GetPlayerView().GetSymbol());
      }
    }
  }
}