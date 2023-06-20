using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Features.Loading
{
  public class LoadingMono : MonoBehaviour
  {
    private const float Duration = 1.5f;
    
    public CanvasGroup CanvasGroup;
    public Transform Icon;

    public void Construct(Action onEnd) => 
      Animate(onEnd);

    private void Animate(Action onEnd)
    {
      CanvasGroup.alpha = 0;

      int count = Random.Range(1, 4);
      Icon.DORotate(new Vector3(0, 0, 360), Duration)
        .SetRelative(true)
        .SetLoops(count, LoopType.Restart)
        .OnComplete(() => onEnd?.Invoke());

      CanvasGroup.DOFade(1, 0.3f).SetDelay(0.2f);
    }
  }
}