using _Project.Scripts.Features.GameScreen.Mono;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Features.Player.Bots;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;

namespace _Project.Scripts.Features.GameScreen
{
  public class BotGameViewLogic : GameViewLogic
  {
    private readonly Bot _bot;
    private readonly PreparingMono _preparingMono;

    public BotGameViewLogic(GameViewMono gameViewMono, PreparingMono preparingMono, IGameFactory gameFactory, ILocaleSystem localeSystem,
      IGameMetaProvider gameMetaProvider, IAudioService audioService, IInputResolver inputResolver, IAssetProvider assetProvider, 
      IGameSaveSystem gameSaveSystem)
      : base(gameViewMono, gameFactory, localeSystem, gameMetaProvider, audioService, inputResolver, assetProvider, gameSaveSystem)
    {
      _preparingMono = preparingMono;
      
      _bot = GameMetaProvider.GetGame() == Game.Classic ? new ClassicBot(gameFactory, gameMetaProvider) : new BuhurtBot(gameFactory, gameMetaProvider, FiguresState);
      _bot.Construct();
    }

    public override void StartGame()
    {
      _preparingMono.Construct(GameMetaProvider, LocaleSystem, () =>
      {
        GameFactory.GameFieldLogic.Show(null);
        InputResolver.Enable();
        
        GameFactory.GameFieldLogic.Show(() =>
        {
          if (GameMetaProvider.GetPlayerView().GetSymbol() == SymbolSign.O)
            BotTurn();
        });
      });
    }

    public override void ChangeTurningPlayer(SymbolSign activeSign)
    {
      base.ChangeTurningPlayer(activeSign);

      if (GameMetaProvider.GetEnemyType() == EnemyType.Bot && !InputResolver.IsLocked)
        BotTurn();
    }

    private void BotTurn()
    {
      InputResolver.Disable();
      _bot?.BotTurn(OnTurnEnd, () => { OnEndGame?.Invoke(SymbolSign.None); });

      void OnTurnEnd()
      {
        InputResolver.Enable();
        RefreshAfterChangeTurn(GameMetaProvider.GetPlayerView().GetSymbol());
      }
    }
  }
}