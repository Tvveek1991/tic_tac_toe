using System;
using _Project.Scripts.Features.GameScreen.Serialized;

namespace _Project.Scripts.Features.GameScreen
{
  public interface IGameViewLogic
  {
    void Construct(Action onClickBack, Action onCreateNonConnectionPlayer = null);
    void OpenWindow();
    void SetPlayerUi(SymbolSign symbol);
    void ChangeTurningPlayer(SymbolSign activeSign = SymbolSign.None);
  }
}