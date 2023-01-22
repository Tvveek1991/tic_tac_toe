using System;
using UnityEngine;
using _Project.Scripts.Features.GameScreen.Serialize;

namespace _Project.Scripts.Features.Games
{
  public class SymbolCell : MonoBehaviour
  {
    private MeshRenderer _meshRenderer;
    
    private SymbolSign _symbolSign;
    private Action<SymbolSign> _onClickCell;

    public void Construct(Action<SymbolSign> onClickCell)
    {
      _onClickCell = onClickCell;
      
      _meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void SetSymbol(SymbolSign symbol)
    {
      _symbolSign = symbol;
      _meshRenderer.enabled = true;
      
      _onClickCell.Invoke(symbol);
    }

    public SymbolSign GetSymbol =>
      _symbolSign;
  }
}