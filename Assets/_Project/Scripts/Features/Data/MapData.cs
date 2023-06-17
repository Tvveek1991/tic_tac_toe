using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Features.Data
{
  [CreateAssetMenu(fileName = "Map Data", menuName = "Data/Playable/Map Data", order = 0)]
  public class MapData : GameCustomData
  {
    [SerializeField] private Mesh _mesh;
    [SerializeField] private List<Texture> _textures;
    
    public Mesh FieldMesh => _mesh;
    public List<Texture> Textures => _textures;
  }
}