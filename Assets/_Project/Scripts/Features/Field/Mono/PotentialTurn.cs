using UnityEngine;

namespace _Project.Scripts.Features.Field.Mono
{
  public class PotentialTurn : MonoBehaviour
  {
    private readonly int _alpha = Shader.PropertyToID("_Alpha");
    
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
      _meshRenderer.material = material;
      _meshRenderer.material.SetFloat(_alpha, .2f);
      
      transform.localRotation = Quaternion.Euler(angle);
      
      _meshRenderer.enabled = true;
    }

    public void Hide() => 
      _meshRenderer.enabled = false;
  }
}