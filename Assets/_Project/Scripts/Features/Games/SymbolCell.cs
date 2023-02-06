using System;
using System.Collections.Generic;
using _Project.Scripts.Features.Data;
using UnityEngine;
using _Project.Scripts.Features.GameScreen.Serialize;
using _Project.Scripts.Services.GameMetaProvider;
using Unity.Netcode;

namespace _Project.Scripts.Features.Games
{
  public class SymbolCell : NetworkBehaviour
  {
    private readonly NetworkVariable<SymbolSign> _netSymbol = new NetworkVariable<SymbolSign>();

    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    private Action<SymbolSign> _onClickCell;

    private Dictionary<SymbolSign, PlayerData> _playerData;
    
    public void Construct(Action<SymbolSign> onClickCell, Dictionary<SymbolSign, PlayerData> playerData)
    {
      _onClickCell = onClickCell;
      _playerData = playerData;

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
      _meshFilter.mesh = _playerData[next].SymbolData.Mesh;
      _meshRenderer.material = _playerData[next].SymbolData.Material;
      _meshRenderer.material.color = _playerData[next].Color;
      _meshRenderer.enabled = true;

      _onClickCell.Invoke(next);
    }
  }
}