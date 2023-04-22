using System;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.GameScreen.Serialized;
using Unity.Netcode;
using UnityEngine;

namespace _Project.Scripts.Features.Field.Mono
{
  public class SymbolCell : NetworkBehaviour
  {
    private readonly NetworkVariable<SymbolSign> _netSymbol = new NetworkVariable<SymbolSign>();
    private readonly NetworkVariable<SymbolSize> _netSize = new NetworkVariable<SymbolSize>(SymbolSize.Default);

    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    private Action<DragableData> _onClickCell;

    public MeshFilter MeshFilter => _meshFilter;
    public MeshRenderer MeshRenderer => _meshRenderer;

    public void Construct(Action<DragableData> onClickCell)
    {
      _onClickCell = onClickCell;

      _meshFilter = GetComponentsInChildren<MeshFilter>()[1];
      _meshRenderer = GetComponentsInChildren<MeshRenderer>()[1];
    }

    public SymbolSign GetSymbol() =>
      _netSymbol.Value;

    public SymbolSize GetSize() =>
      _netSize.Value;

    public void SetSymbol(DragableData dragableData)
    {
      _netSymbol.Value = dragableData.GetSymbol();
      _netSize.Value = dragableData.GetSize();
      _onClickCell.Invoke(dragableData);
    }

    #region Multiplayer

    public override void OnNetworkSpawn()
    {
      _netSymbol.OnValueChanged += OnValueSymbolChanged;
      _netSize.OnValueChanged += OnValueSizeChanged;
    }

    public override void OnDestroy()
    {
      _netSymbol.OnValueChanged -= OnValueSymbolChanged;
      _netSize.OnValueChanged -= OnValueSizeChanged;
    }


    [ServerRpc(RequireOwnership = false)]
    public void SetSymbolServerRpc(SymbolSign symbol) =>
      _netSymbol.Value = symbol;

    [ServerRpc(RequireOwnership = false)]
    public void SetSizeServerRpc(SymbolSize size) =>
      _netSize.Value = size;

    private void OnValueSymbolChanged(SymbolSign previous, SymbolSign next) =>
      _onClickCell.Invoke(null);

    private void OnValueSizeChanged(SymbolSize previous, SymbolSize next) =>
      Debug.Log("Size is changes");

    #endregion
  }
}