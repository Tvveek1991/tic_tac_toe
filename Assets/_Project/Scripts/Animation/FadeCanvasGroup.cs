using System;
using _Project.Scripts.MonoBehaviours.UI;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace _Project.Scripts.Animations.UI
{
  public class FadeCanvasGroup
  {
    private const float Duration = 0.25f;
    
    private readonly CanvasGroup _canvasGroup;

    private readonly float _delay;
    private readonly float _duration;

    private TweenerCore<float, float, FloatOptions> _tweenAppear;
    private TweenerCore<float, float, FloatOptions> _tweenDisappear;

    public FadeCanvasGroup(CanvasGroup canvasGroup, float delay = 0, float duration = Duration)
    {
      var objectWithAnimation = canvasGroup.gameObject.AddComponent<ObjectWithAnimation>();
      objectWithAnimation.Construct(() =>
      {
        _tweenAppear?.Kill();
        _tweenDisappear?.Kill();
      });
      
      _duration = duration;
      _delay = delay;
      _canvasGroup = canvasGroup;
      SetupBeforeOpen();
    }

    public void Appear(Action onStart = null, Action onEnd = null, float startValue = 0, float target = 1)
    {  
      if (_canvasGroup == null)
      {
        onStart?.Invoke();
        onEnd?.Invoke();
        return;
      }

      _canvasGroup.interactable = true;
      _canvasGroup.alpha = startValue;
      _tweenAppear = _canvasGroup.DOFade(target, _duration).SetEase(Ease.Linear).SetDelay(_delay);
      _tweenAppear.onComplete += () => onEnd?.Invoke();
      onStart?.Invoke();
    }

    public void Disappear(Action onStart = null, Action onEnd = null, float targetFade = 0f)
    {
      if (_canvasGroup == null)
      {
        onStart?.Invoke();
        onEnd?.Invoke();
        return;
      }
      
      _canvasGroup.interactable = false;
      _tweenDisappear = _canvasGroup.DOFade(targetFade, _duration).SetEase(Ease.Linear);
      _tweenDisappear.onComplete += () => onEnd?.Invoke();
      onStart?.Invoke();
    }

    public void Clear()
    {
      _tweenAppear?.Kill();
      _tweenDisappear?.Kill();

      _tweenAppear = null;
      _tweenDisappear = null;
    }
    
    private void SetupBeforeOpen() => 
      _canvasGroup.alpha = 0;
  }
}