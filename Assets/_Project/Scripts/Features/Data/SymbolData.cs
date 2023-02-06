using UnityEngine;

namespace _Project.Scripts.Features.Data
{
  [CreateAssetMenu(fileName = "Symbol Data", menuName = "Data/Playable/Symbol Data", order = 0)]
  public class SymbolData : BaseContentData
  {
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _material;
    [SerializeField] private Sprite _icon;

    public Mesh Mesh => _mesh;
    public Material Material => _material;
    public Sprite Icon => _icon;
  }
}