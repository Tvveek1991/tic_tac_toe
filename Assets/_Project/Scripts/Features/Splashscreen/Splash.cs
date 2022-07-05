using System;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Features.Splashscreen
{
  public class Splash : MonoBehaviour
  {
    private const float SplashTime = 5f;
    
    public CanvasGroup LogoGroup;
    public Transform LogoTransform;
    private Action _onEnd;
    
    private bool _isComplete;
    private bool _isAnimationComplete;

    public void Construct(Action onEnd) => 
      _onEnd = onEnd;

    public void Complete()
    {
      _isComplete = true;

      if (_isAnimationComplete) 
        _onEnd?.Invoke();
    }

    public void Launch()
    {
      LogoTransform.localScale = 0.7f * Vector3.one;
      LogoGroup.alpha = 0;
      
      LogoGroup.DOFade(1, SplashTime);
      LogoTransform.DOScale(1, SplashTime).onComplete += () =>
      {
        _isAnimationComplete = true;

        if (_isComplete) 
          _onEnd?.Invoke();
      };
    }
  }
}
