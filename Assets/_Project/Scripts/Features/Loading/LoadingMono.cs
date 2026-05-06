using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Features.Loading
{
  public class LoadingMono : MonoBehaviour
  {
    private const float Time = .5f;
    private const float FadeDuration = .25f;

    public CanvasGroup CanvasGroup;

    [SerializeField] private List<Transform> fieldSymbol;

    public void Construct(Action onLoad, Action onEnd) =>
      Animate(onLoad, onEnd);

    private void Animate(Action onLoad, Action onEnd)
    {
      CanvasGroup.alpha = 0;
      CanvasGroup.DOFade(1, FadeDuration).SetDelay(0.2f)
        .OnComplete(() => onLoad?.Invoke());

      var startValue = 0f;
      DOTween.To(() => startValue, x => startValue = x, 1f, Time)
        .SetLoops(Random.Range(2, 8), LoopType.Restart)
        .SetEase(Ease.Linear)
        .OnStart(Animate)
        .OnStepComplete(Animate)
        .OnComplete(() => Hide(onEnd));
    }

    private void Hide(Action onEnd)
    {
      CanvasGroup.DOFade(0, FadeDuration)
        .OnComplete(() => onEnd?.Invoke());
    }

    private void Animate()
    {
      while (true)
      {
        int rnd = Random.Range(0, fieldSymbol.Count);
        if (DOTween.IsTweening(fieldSymbol[rnd]))
          continue;

        fieldSymbol[rnd]
          .DOScale(Vector3.one, .5f)
          .SetEase(Ease.Linear)
          .SetLoops(2, LoopType.Yoyo)
          .OnComplete(() => { fieldSymbol[rnd].DOKill(); });
        break;
      }
    }
  }
}