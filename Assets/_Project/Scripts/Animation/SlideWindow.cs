using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Animations.UI
{
  public class SlideWindow
  {
    private const float Duration = .45f;
    
    private readonly RectTransform _transform;

    public SlideWindow(RectTransform transform)
    {
      _transform = transform;
      
      SetupBeforeOpen();
    }

    public async void Open(Action onStart = null, Action onEnd = null)
    {
      await Task.Delay(200);
      
      SetupBeforeOpen();

      _transform.DOAnchorPosY(-_transform.sizeDelta.y, Duration).SetEase(Ease.OutExpo).onComplete += () => onEnd?.Invoke();
      onStart?.Invoke();
    }

    public void Close(Action onStart = null, Action onEnd = null)
    {
      _transform.DOAnchorPosY(0, Duration).SetEase(Ease.OutExpo).onComplete += () => onEnd?.Invoke();
      onStart?.Invoke();
    }

    private void SetupBeforeOpen() => 
      _transform.anchoredPosition = new Vector2( _transform.anchoredPosition.x, 0);
  }
}