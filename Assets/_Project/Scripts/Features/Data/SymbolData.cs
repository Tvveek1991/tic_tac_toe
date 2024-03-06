using System.Collections.Generic;
using _Project.Scripts.Features.ShopScreen.Serialized;
using UnityEngine;

namespace _Project.Scripts.Features.Data
{
  [CreateAssetMenu(fileName = "Symbol Data", menuName = "Data/Playable/Symbol Data", order = 0)]
  public class SymbolData : GameCustomData
  {
    [SerializeField] private Mesh _meshClassic;
    [SerializeField] private Material _materialClassic;
    
    [SerializeField] private List<Mesh> _meshBuhurt;
    [SerializeField] private List<Material> _materialBuhurt;
    [SerializeField] private List<Vector3> _rotateAngle;
    [SerializeField] private Vector3 _symbolPosition;
    [SerializeField] private Vector3 _animationAngle;
    
    [SerializeField] private Sprite _icon;
    [SerializeField] private bool _isBackOn;
    
    public Mesh MeshClassic => _meshClassic;
    public Material MaterialClassic => _materialClassic;
    public Sprite Icon => _icon;
    public List<Mesh> MeshBuhurt => _meshBuhurt;
    public List<Material> MaterialBuhurt => _materialBuhurt;
    public List<Vector3> RotateAngle => _rotateAngle;
    public Vector3 SymbolPosition => _symbolPosition;
    public Vector3 AnimationAngle => _animationAngle;
    public bool IsBackOn => _isBackOn;
  }
}