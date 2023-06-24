using _Project.Scripts.Features.Field.Mono;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Features.Player.BuhurtModules;
using UnityEngine;

namespace _Project.Scripts.Features.Player
{
  public class BuhurtPlayerViewMono : BasePlayerViewMono
  {
    private DragablePart _dragablePart;
    
    private void OnMouseUp()
    {
      if(InputResolver.IsLocked)
        return;
      
      if (!IsPlayerTurn)
        return;

      _dragablePart = GameFactory.GameViewLogic.DragDrop?.CurrentActiveDragPart;
      
      Ray ray = Camera.ScreenPointToRay(Application.isEditor ? Input.mousePosition : Input.touches[0].position);

      if (!Physics.Raycast(ray, out RaycastHit hit) || _dragablePart == null)
        return;

      var cell = hit.transform.gameObject.GetComponent<SymbolCell>();

      if (!_dragablePart.CanPin(cell.GetSize(), cell.GetSymbol()))
        return;

      MakeMove(cell);
    }

    private void MakeMove(SymbolCell cell)
    {
      _dragablePart.PinPart();
      GameFactory.GameViewLogic.SetFigureState(GetSymbol(), _dragablePart.GetDragableData().GetSize());
      
      cell.SetSymbol(_dragablePart.GetDragableData());
      
      GameFactory.GameViewLogic.ChangeTurningPlayer(GetSymbol());
    }
  }
}