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
    private readonly ClassicBot _classicBot;

    private readonly IGameFactory _gameFactory;
    
    public SingleGameViewLogic(GameViewMono gameViewMono, IGameFactory gameFactory, ILocaleSystem localeSystem, IGameMetaProvider gameMetaProvider, IAudioService audioService)
      : base(gameViewMono, localeSystem, gameMetaProvider, audioService)
    {
      _gameFactory = gameFactory;
      _classicBot = new ClassicBot(_gameFactory);
    }

    public override void Construct(Action onBack, Action onCreateNonConnectionPlayer)
    {
      base.Construct(onBack, onCreateNonConnectionPlayer);
      
      OnCreateNonConnectionPlayer?.Invoke();
      
      GameMetaProvider.GetPlayerViewMono().Construct(_gameFactory, GameMetaProvider); 
      GameMetaProvider.GetPlayerViewMono().CreateSymbolSign(0);
      
      GameViewMono.HideLoadScreen();
      RefreshAfterChangeTurn(SymbolSign.X);
    }

    public override void ChangeTurningPlayer(SymbolSign activeSign)
    {
      SymbolSign nextSign = activeSign == SymbolSign.X ? SymbolSign.O : SymbolSign.X;
      RefreshAfterChangeTurn(nextSign);

      if (GameMetaProvider.GetEnemyType() == EnemyType.Bot)
        _classicBot.BotTurn(() => RefreshAfterChangeTurn(activeSign));
      else if (GameMetaProvider.GetEnemyType() == EnemyType.OneDevice) 
        GameMetaProvider.GetPlayerViewMono().CreateSymbolSign((int)nextSign - 1);
    }

    private void RefreshAfterChangeTurn(SymbolSign nextSign)
    {
      SetPlayerUi(nextSign);
      GameMetaProvider.GetPlayerViewMono().SetPlayerTurn(GameMetaProvider.GetEnemyType() == EnemyType.OneDevice || nextSign == SymbolSign.X);
      GameViewMono.TurnText.text = nextSign == SymbolSign.X ? "Player X" : "Player O";
    }
  }
}