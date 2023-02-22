using System;
using _Project.Scripts.Features.GameScreen.Mono;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Network;

namespace _Project.Scripts.Features.GameScreen
{
  public class SingleGameViewLogic : GameViewLogic
  {
    public SingleGameViewLogic(GameViewMono gameViewMono, ILocaleSystem localeSystem, INetwork network, IGameMetaProvider gameMetaProvider, IAudioService audioService)
      : base(gameViewMono, localeSystem, network, gameMetaProvider, audioService)
    {
    }

    public override void Construct(Action onBack, Action onCreateNonConnectionPlayer)
    {
      base.Construct(onBack, onCreateNonConnectionPlayer);

      GameViewMono.Construct(null, null, null);
    }
  }
}