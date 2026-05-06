using UnityEngine;

namespace _Project.Scripts.Features.Field.Mono
{
  public class PotentialTurn : MonoBehaviour
  {
    private readonly int _color = Shader.PropertyToID("_Color");
    private readonly int _mainTex = Shader.PropertyToID("_MainTex");
    
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    

    public void Construct()
    {
      _meshFilter = GetComponentInChildren<MeshFilter>();
      _meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void ShowPotentialTurn(Mesh mesh, Material material, Vector3 angle)
    {
      _meshFilter.mesh = mesh;
      _meshRenderer.material.SetTexture(_mainTex, material.GetTexture(_mainTex));

      Color color = material.GetColor(_color);
      color.a = .4f;
      _meshRenderer.material.SetColor(_color, color);
      
      transform.localRotation = Quaternion.Euler(angle);
      
      _meshRenderer.enabled = true;
    }

    public void Hide() => 
      _meshRenderer.enabled = false;
  }
}