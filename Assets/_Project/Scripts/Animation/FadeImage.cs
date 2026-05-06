using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Animation
{
  public class FadeImage
  {
    private const float Duration = 0.25f;
    
    private readonly Image _image;
    
    private Tween _appear;
    private Tween _disappear;

    public FadeImage(Image image)
    {
      _image = image;
      SetupBeforeOpen();
    } 

    public void Appear(Action onStart = null, Action onEnd = null)
    {
      _appear = _image.DOFade(1, Duration);
      _appear.onComplete += () => onEnd?.Invoke();
      onStart?.Invoke();
    }

    public void Disappear(Action onStart = null, Action onEnd = null)
    {
      _disappear = _image.DOFade(0, Duration);
      _disappear.onComplete += () => onEnd?.Invoke();
      onStart?.Invoke();
    }

    public void Clear()
    {
      _appear?.Kill();
      _disappear?.Kill();

      _appear = null;
      _disappear = null;
    }

    private void SetupBeforeOpen() => 
      _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0);
  }
}