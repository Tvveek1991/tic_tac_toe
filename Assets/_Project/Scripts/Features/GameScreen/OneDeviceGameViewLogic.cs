using _Project.Scripts.Features.GameScreen.Mono;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;

namespace _Project.Scripts.Features.GameScreen
{
  public class OneDeviceGameViewLogic : GameViewLogic
  {
    public OneDeviceGameViewLogic(GameViewMono gameViewMono, IGameFactory gameFactory, ILocaleSystem localeSystem, IGameMetaProvider gameMetaProvider,
      IAudioService audioService, IInputResolver inputResolver, IAssetProvider assetProvider, IGameSaveSystem gameSaveSystem)
      : base(gameViewMono, gameFactory, localeSystem, gameMetaProvider, audioService, inputResolver, assetProvider, gameSaveSystem) { }

    public override void ChangeTurningPlayer(SymbolSign activeSign)
    {
      base.ChangeTurningPlayer(activeSign);

      if (GameMetaProvider.GetEnemyType() == EnemyType.OneDevice)
        GameMetaProvider.GetPlayerView().ExchangeSymbolSign();
    }
  }
}