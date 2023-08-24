using UnityEngine;

namespace _Project.Scripts.Features.Field.Mono
{
  public class PotentialTurn : MonoBehaviour
  {
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    
    public void Construct()
    {
      _meshFilter = GetComponentInChildren<MeshFilter>();
      _meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void ShowPotentialTurn(Mesh mesh)
    {
      _meshFilter.mesh = mesh;
      _meshRenderer.enabled = true;
    }

    public void Hide() => 
      _meshRenderer.enabled = false;
  }
}