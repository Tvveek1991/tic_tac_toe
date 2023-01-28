using System;
using System.Collections.Generic;
using UnityEngine;
using _Project.Scripts.Features.GameScreen.Serialize;

namespace _Project.Scripts.Features.Games
{
  public class SymbolCell : MonoBehaviour
  {
    private readonly List<Color> _colors = new List<Color> {Color.red, Color.blue, Color.green, Color.yellow};
    
    private MeshRenderer _meshRenderer;
    
    private SymbolSign _symbolSign;
    private Action<SymbolSign> _onClickCell;

    public void Construct(Action<SymbolSign> onClickCell)
    {
      _onClickCell = onClickCell;
      
      _meshRenderer = GetComponentsInChildren<MeshRenderer>()[1];
    }

    public void SetSymbol(SymbolSign symbol)
    {
      _symbolSign = symbol;
      _meshRenderer.material.color = _colors[(int)symbol - 1];
      _meshRenderer.enabled = true;
      
      _onClickCell.Invoke(symbol);
    }

    public SymbolSign GetSymbol =>
      _symbolSign;
  }
}