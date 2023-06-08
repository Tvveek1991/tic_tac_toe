using System;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Features.ShopScreen.Mono
{
  public class IconPreview : MonoBehaviour
  {
    private const float Duration = .15f;
    
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    public void Construct()
    {
      _meshFilter = transform.GetComponentInChildren<MeshFilter>();
      _meshRenderer = transform.GetComponentInChildren<MeshRenderer>();
    }

    public void Hide(Action onEnd) => 
      transform.DOScale(0f, Duration)
        .OnComplete(() => onEnd?.Invoke());

    public void Show(float factor) => 
      transform.DOScale(1f * factor, Duration);

    public void SetVisual(Mesh mesh)
    {
      _meshFilter.mesh = mesh;
    }
  }
}