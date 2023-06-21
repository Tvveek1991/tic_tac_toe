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
      _bot = GameMetaProvider.GetGame() == Game.Classic ? new ClassicBot(_gameFactory) : new BuhurtBot(_gameFactory);
    }

    public override void Construct(Action onBack, Action onCreateNonConnectionPlayer, Action<SymbolSign> onEndGame)
    {
      base.Construct(onBack, onCreateNonConnectionPlayer, onEndGame);

      OnCreateNonConnectionPlayer?.Invoke();

      GameMetaProvider.GetPlayerViewMono().Construct(_gameFactory, GameMetaProvider, InputResolver);
      GameMetaProvider.GetPlayerViewMono().CreateSymbolSign(0);

      GameViewMono.HideLoadScreen();
      RefreshAfterChangeTurn(SymbolSign.X);
    }

    public override void ChangeTurningPlayer(SymbolSign activeSign)
    {
      SymbolSign nextSign = activeSign == SymbolSign.X ? SymbolSign.O : SymbolSign.X;
      RefreshAfterChangeTurn(nextSign);

      if (GameMetaProvider.GetEnemyType() == EnemyType.Bot && !InputResolver.IsLocked)
        BotTurn(activeSign);
      else if (GameMetaProvider.GetEnemyType() == EnemyType.OneDevice)
        GameMetaProvider.GetPlayerViewMono().CreateSymbolSign((int) nextSign - 1);
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

      bool isPlayerTurn = GameMetaProvider.GetEnemyType() == EnemyType.OneDevice || nextSign == SymbolSign.X;

      GameMetaProvider.GetPlayerViewMono().SetPlayerTurn(isPlayerTurn);
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

    private void BotTurn(SymbolSign activeSign)
    {
      InputResolver.Disable();
      _bot?.BotTurn(OnTurnEnd, () => OnEndGame?.Invoke(SymbolSign.None));

      void OnTurnEnd()
      {
        InputResolver.Enable();
        RefreshAfterChangeTurn(activeSign);
      }
    }
  }
}