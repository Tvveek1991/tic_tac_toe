using System;
using _Project.Scripts.Features.DragAndDrop.Interface;
using _Project.Scripts.Features.GameScreen.Mono;
using _Project.Scripts.Features.GameScreen.Serialized;

namespace _Project.Scripts.Features.GameScreen
{
  public interface IGameViewLogic
  {
    IDragDrop DragDrop { get; }
    void Construct(Action onClickBack, Action onCreateNonConnectionPlayer, Action<SymbolSign> onEndGame);
    void OpenWindow();
    void SetPlayerUi(SymbolSign symbol);
    void ChangeTurningPlayer(SymbolSign activeSign = SymbolSign.None);
    void SetFigureState(SymbolSign symbol, SymbolSize size);
  }
}