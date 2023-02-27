using _Project.Scripts.Features.Field.Mono;
using _Project.Scripts.Features.GameScreen.Serialized;
using UnityEngine;

namespace _Project.Scripts.Features.Player
{
  public class ClassicPlayerViewMono : BasePlayerViewMono
  {
    private void OnMouseDown()
    {
      if (!IsOwner || !NetIsPlayerTurn.Value)
        return;

      Ray ray = Camera.ScreenPointToRay(Application.isEditor ? Input.mousePosition : (Vector3) Input.touches[0].position);

      if (!Physics.Raycast(ray, out RaycastHit hit))
        return;

      var cell = hit.transform.gameObject.GetComponent<SymbolCell>();

      if (cell.GetSymbol != SymbolSign.None)
        return;

      cell.SetSymbolServerRpc(NetSymbolSign.Value);
      GameFactory.GameViewLogic.ChangeTurningPlayerServerRpc();
    }
  }
}