using _Project.Scripts.Features.GameScreen.Serialized;
using UnityEngine;

namespace _Project.Scripts.Features.Data
{
  public class DragableData
  {
    private readonly SymbolSign _sign;
    private readonly SymbolSize _size;
    private readonly Mesh _mesh;
    private readonly Material _material;

    public DragableData(SymbolSign sign, SymbolSize size, Mesh mesh, Material material)
    {
      _sign = sign;
      _size = size;
      _mesh = mesh;
      _material = material;
    }

    public SymbolSign GetSymbol() =>
      _sign;

    public SymbolSize GetSize() =>
      _size;

    public Mesh GetMesh() =>
      _mesh;

    public Material GetMaterial() =>
      _material;
  }
}