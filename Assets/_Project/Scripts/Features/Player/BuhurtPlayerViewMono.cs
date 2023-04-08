using _Project.Scripts.Features.Field.Mono;
using _Project.Scripts.Features.GameScreen.Serialized;
using UnityEngine;

namespace _Project.Scripts.Features.Player
{
  public class BuhurtPlayerViewMono : BasePlayerViewMono
  {
    private void OnMouseUp()
    {
      if ((!IsOwner && GameMetaProvider.GetGameMode() != GameMode.Single) || !NetIsPlayerTurn.Value)
        return;
      
      Ray ray = Camera.ScreenPointToRay(Application.isEditor ? Input.mousePosition : (Vector3) Input.touches[0].position);

      if (!Physics.Raycast(ray, out RaycastHit hit) || GameFactory.GameViewLogic.DragDrop?.CurrentActiveDragPart == null)
        return;

      var cell = hit.transform.gameObject.GetComponent<SymbolCell>();

      if (cell.GetSymbol() != SymbolSign.None)
        return;

      if (GameMetaProvider.GetGameMode() == GameMode.Single)
        cell.SetSymbol(NetSymbolSign.Value);
      else
        cell.SetSymbolServerRpc(NetSymbolSign.Value);

      GameFactory.GameViewLogic.ChangeTurningPlayer(GetSymbol());
    }
  }
}