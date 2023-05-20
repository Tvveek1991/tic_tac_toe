using System.Collections.Generic;
using _Project.Scripts.Features.ShopScreen.Serialized;
using UnityEngine;

namespace _Project.Scripts.Features.Data
{
  [CreateAssetMenu(fileName = "Symbol Data", menuName = "Data/Playable/Symbol Data", order = 0)]
  public class SymbolData : BaseContentData
  {
    [SerializeField] private Mesh _meshClassic;
    [SerializeField] private Material _materialClassic;
    
    [SerializeField] private List<Mesh> _meshBuhurt;
    [SerializeField] private List<Material> _materialBuhurt;
    
    [SerializeField] private Sprite _icon;
    
    [SerializeField] private ItemType _itemType;

    public ItemType ItemType => _itemType;
    public Mesh MeshClassic => _meshClassic;
    public Material MaterialClassic => _materialClassic;
    public Sprite Icon => _icon;
    public List<Mesh> MeshBuhurt => _meshBuhurt;
    public List<Material> MaterialBuhurt => _materialBuhurt;
  }
}