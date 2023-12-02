using System;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Features.Loading
{
  public class LoadingMono : MonoBehaviour
  {
    private const float FadeDuration = .5f;
    private const float Duration = 2f;
    
    public CanvasGroup CanvasGroup;
    public Transform Icon;

    public void Construct(Action onLoad, Action onEnd) => 
      Animate(onLoad, onEnd);

    private void Animate(Action onLoad, Action onEnd)
    {
      CanvasGroup.alpha = 0;

      int count = 2;// Random.Range(1, 4);
      Icon.DORotate(new Vector3(0, 0, 360), Duration)
        .SetRelative(true)
        .SetLoops(count, LoopType.Restart)
        .OnComplete(() =>
        {
          CanvasGroup.DOFade(0, FadeDuration)
            .OnComplete(() => onEnd?.Invoke());
        });

      CanvasGroup.DOFade(1, FadeDuration).SetDelay(0.2f)
        .OnComplete(() => onLoad?.Invoke());
    }
  }
}