using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.Field.Mono;
using _Project.Scripts.Features.GameScreen.Serialized;
using UnityEngine;

namespace _Project.Scripts.Features.Player
{
  public class ClassicPlayerViewMono : BasePlayerViewMono
  {
    private DragableData _dragableData;

    private void OnMouseDown()
    {
      if(InputResolver.IsLocked)
        return;
      
      _dragableData = new DragableData(SymbolSign, SymbolSize.Default);

      if (!IsPlayerTurn)
        return;

      Ray ray = Camera.ScreenPointToRay(Application.isEditor ? Input.mousePosition : Input.touches[0].position);

      if (!Physics.Raycast(ray, out RaycastHit hit))
        return;

      var cell = hit.transform.gameObject.GetComponent<SymbolCell>();

      if (cell.GetSymbol() != SymbolSign.None)
        return;

      cell.SetSymbol(_dragableData);

      GameFactory.GameViewLogic.ChangeTurningPlayer(GetSymbol());
    }
  }
}