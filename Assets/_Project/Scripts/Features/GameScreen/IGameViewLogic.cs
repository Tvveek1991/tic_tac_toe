using System;
using _Project.Scripts.Services._Interfaces;

namespace _Project.Scripts.Features.GameScreen
{
  public interface IGameViewLogic
  {
    void Construct(IAudioService audioService, Action onClickBack);
    void OpenWindow();

  }
}