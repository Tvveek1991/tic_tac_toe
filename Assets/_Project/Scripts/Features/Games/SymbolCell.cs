using System;
using UnityEngine;
using _Project.Scripts.Features.GameScreen.Serialize;
using Unity.Netcode;

namespace _Project.Scripts.Features.Games
{
  public class SymbolCell : NetworkBehaviour
  {
    private readonly NetworkVariable<SymbolSign> _netSymbol = new NetworkVariable<SymbolSign>();

    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;

    private Action<SymbolSign> _onClickCell;

    public MeshFilter MeshFilter => _meshFilter;
    public MeshRenderer MeshRenderer => _meshRenderer;
    
    public void Construct(Action<SymbolSign> onClickCell)
    {
      _onClickCell = onClickCell;

      _meshFilter = GetComponentsInChildren<MeshFilter>()[1];
      _meshRenderer = GetComponentsInChildren<MeshRenderer>()[1];
    }

    public override void OnNetworkSpawn() => 
      _netSymbol.OnValueChanged += OnValueChanged;

    public override void OnDestroy() => 
      _netSymbol.OnValueChanged -= OnValueChanged;

    [ServerRpc(RequireOwnership = false)] //Because owner of object is Host we need stand RequireOwnership at this attribute
    public void SetSymbolServerRpc(SymbolSign symbol) =>
      _netSymbol.Value = symbol;

    public SymbolSign GetSymbol =>
      _netSymbol.Value;
    
    private void OnValueChanged(SymbolSign previous, SymbolSign next) =>
      _onClickCell.Invoke(next);
  }
}