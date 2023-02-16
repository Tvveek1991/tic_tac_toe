using System;
using _Project.Scripts.Features.GameScreen.Serialize;
using _Project.Scripts.Services._Interfaces;

namespace _Project.Scripts.Features.GameScreen
{
  public interface IGameViewLogic
  {
    void Construct(IAudioService audioService, Action onClickBack, Action onCreateNonConnectionPlayer = null);
    void OpenWindow();
    void SetPlayerUi(SymbolSign symbol);
    void ChangeTurningPlayerServerRpc();
  }
}