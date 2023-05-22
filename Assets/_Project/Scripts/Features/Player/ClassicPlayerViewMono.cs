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
      
      _dragableData = new DragableData(NetSymbolSign.Value, SymbolSize.Default);

      if ((!IsOwner && GameMetaProvider.GetGameMode() != GameMode.Single) || !NetIsPlayerTurn.Value)
        return;

      Ray ray = Camera.ScreenPointToRay(Application.isEditor ? Input.mousePosition : Input.touches[0].position);

      if (!Physics.Raycast(ray, out RaycastHit hit))
        return;

      var cell = hit.transform.gameObject.GetComponent<SymbolCell>();

      if (cell.GetSymbol() != SymbolSign.None)
        return;

      if (GameMetaProvider.GetGameMode() == GameMode.Single)
        cell.SetSymbol(_dragableData);
      else
        cell.SetSymbolServerRpc(_dragableData.GetSymbol());

      GameFactory.GameViewLogic.ChangeTurningPlayer(GetSymbol());
    }
  }
}