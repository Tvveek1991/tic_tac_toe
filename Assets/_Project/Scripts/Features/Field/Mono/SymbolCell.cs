using System;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.GameScreen.Serialized;
using UnityEngine;

namespace _Project.Scripts.Features.Field.Mono
{
  public class SymbolCell : MonoBehaviour
  {
    private SymbolSign _symbol;
    private SymbolSize _size = SymbolSize.Default;

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
      _symbol;

    public SymbolSize GetSize() =>
      _size;

    public void SetSymbol(DragableData dragableData)
    {
      _symbol = dragableData.GetSymbol();
      _size = dragableData.GetSize();
      _onClickCell.Invoke(dragableData);
    }
  }
}