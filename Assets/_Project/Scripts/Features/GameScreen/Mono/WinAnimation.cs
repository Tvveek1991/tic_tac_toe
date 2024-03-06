using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Features.GameScreen.Mono
{
  public class WinAnimation : MonoBehaviour
  {
    private const int ZPos = -4;
    private const float Duration = .3f;
    
    private readonly int _color = Shader.PropertyToID("_Color");
    
    private Material _defaultMaterial;

    private LineRenderer _lineRenderer;

    private Vector3 _startPos;
    private Vector3 _endPos;

    public void Construct(List<Transform> wonCells, Action onEnd, bool isPlayerWin)
    {
      _startPos = new Vector3(wonCells[0].position.x, wonCells[0].position.y, ZPos);
      _endPos = new Vector3(wonCells[2].position.x, wonCells[2].position.y, ZPos);
      
      _defaultMaterial = new Material(Shader.Find("Sprites/Default"));
      _defaultMaterial.SetColor(_color, isPlayerWin ? new Color32(96, 223, 109, 255) : new Color32(255, 83, 83, 255));
      
      _lineRenderer = gameObject.AddComponent<LineRenderer>();
      _lineRenderer.startWidth = .25f;
      Vector3[] positions = {_startPos, _startPos};
      _lineRenderer.SetPositions(positions);
      _lineRenderer.material = _defaultMaterial;

      for (var i = 0; i < wonCells.Count; i++)
      {
        int i1 = i;
        wonCells[i].DOScale(1.1f, Duration)
          .SetDelay(i * Duration)
          .SetLoops(2, LoopType.Yoyo)
          .SetEase(Ease.Linear)
          .OnComplete(() =>
          {
            if(i1 != 2)
              return;
            
            DOTween.To(() => _startPos, x => _startPos = x, _endPos, .75f)
              .SetEase(Ease.Linear)
              .OnUpdate(() =>  _lineRenderer.SetPosition(0, _startPos))
              .OnComplete(() =>
              {
                onEnd?.Invoke();
                Debug.Log("Анимация завершена!");
              });
          });
      }
    }
  }
}