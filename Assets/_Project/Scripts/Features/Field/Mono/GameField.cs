using System.Collections.Generic;
using _Project.Scripts.Features.Data;
using UnityEngine;

namespace _Project.Scripts.Features.Field.Mono
{
  public class GameField : MonoBehaviour
  {
    private readonly int _main = Shader.PropertyToID("_MainTex");
    
    [SerializeField] private Texture _transparentTexture;

    [SerializeField] private MeshFilter _meshFilterField;
    [SerializeField] private MeshRenderer _meshRendererField;

    [SerializeField] private List<SymbolCell> PlayablePoints;

    public List<SymbolCell> GetPlayablePoint =>
      PlayablePoints;

    public void SetField(MapData mapData)
    {
      _meshFilterField.mesh = mapData.FieldMesh;

      for (var i = 0; i < _meshRendererField.materials.Length; i++)
      {
        Texture texture = i < mapData.Textures.Count ? mapData.Textures[i] : _transparentTexture;
        _meshRendererField.materials[i].SetTexture(_main, texture);
      }
    }
  }
}