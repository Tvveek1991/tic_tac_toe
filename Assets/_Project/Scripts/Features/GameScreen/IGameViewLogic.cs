using System;
using _Project.Scripts.Services._Interfaces;

namespace _Project.Scripts.Features.GameScreen
{
  public interface IGameViewLogic
  {
    void Construct(IAudioService audioService, IGameMetaProvider gameMetaProvider, Action onClickBack, Action onLoadComplete);
    void OpenWindow();
  }
}