using System;
using _Project.Scripts.Features.DragAndDrop.Interface;
using _Project.Scripts.Features.GameScreen.Serialized;

namespace _Project.Scripts.Features.GameScreen
{
  public interface IGameViewLogic
  {
    IDragDrop DragDrop { get; }
    void Construct(Action onClickBack, Action<SymbolSign> onEndGame);
    void OpenWindow();
    void ChangeTurningPlayer(SymbolSign activeSign = SymbolSign.None);
    void SetFigureState(SymbolSign symbol, SymbolSize size);
    void HideBottomPanel();
  }
}