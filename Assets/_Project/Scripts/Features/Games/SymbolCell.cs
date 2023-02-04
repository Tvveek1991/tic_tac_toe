using System;
using System.Collections.Generic;
using UnityEngine;
using _Project.Scripts.Features.GameScreen.Serialize;
using Unity.Netcode;

namespace _Project.Scripts.Features.Games
{
  public class SymbolCell : NetworkBehaviour
  {
    private readonly NetworkVariable<SymbolSign> _netSymbol = new NetworkVariable<SymbolSign>();
    
    private readonly List<Color> _colors = new List<Color> {Color.red, Color.blue, Color.green, Color.yellow};

    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    private Action<SymbolSign> _onClickCell;

    public List<Mesh> MeshFilters = new List<Mesh>();
    
    public void Construct(Action<SymbolSign> onClickCell)
    {
      _onClickCell = onClickCell;

      _meshFilter = GetComponentsInChildren<MeshFilter>()[1];
      _meshRenderer = GetComponentsInChildren<MeshRenderer>()[1];
    }

    public override void OnNetworkSpawn()
    {
      _netSymbol.OnValueChanged += OnValueChanged;
    }
    
    public override void OnDestroy()
    {
      _netSymbol.OnValueChanged -= OnValueChanged;
    }

    [ServerRpc(RequireOwnership = false)] //Because owner of object is Host we need stand RequireOwnership at this attribute
    public void SetSymbolServerRpc(SymbolSign symbol)
    {
      _netSymbol.Value = symbol;
    }

    public SymbolSign GetSymbol =>
      _netSymbol.Value;
    
    private void OnValueChanged(SymbolSign previous, SymbolSign next)
    {
      _meshFilter.mesh = MeshFilters[(int) next - 1];
      _meshRenderer.material.color = _colors[(int) next - 1];
      _meshRenderer.enabled = true;

      _onClickCell.Invoke(next);
    }
  }
}