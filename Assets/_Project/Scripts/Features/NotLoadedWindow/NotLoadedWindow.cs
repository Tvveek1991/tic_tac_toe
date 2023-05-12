using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Animation;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Localization.Mono;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.NotLoadedWindow
{
  public class NotLoadedWindow : MonoBehaviour
  {
    private const float Duration = 0.5f;
    
    public Button OkBtn;
    
    [Header("Animations")]
 
    public CanvasGroup Icon;

    public CanvasGroup Background;
    public Transform BodyTransform;
    public CanvasGroup BodyCanvasGroup;
    
    private Action _clear;

    public void Construct(ILocaleSystem localeSystem, Action onClose)
    {
      new ButtonAnimation(OkBtn.transform);
      
      Blink();
      Localize(localeSystem);
      
      OkBtn.onClick.AddListener(onClose.Invoke);
    }

    private void Localize(ILocaleSystem localeSystem)
    {
      List<LocalizedMainText> localizedTexts = transform.GetComponentsInChildren<LocalizedMainText>(true).ToList();
      localizedTexts.ForEach(item =>
      {
        item.Construct(localeSystem);
        item.Localize();
      });
    }

    public void Open() => 
      AppearAnimation();

    public void Close(Action clear) => 
      DisappearAnimation(clear);

    private void Blink()
    {
      var sequence = DOTween.Sequence();
      sequence.SetDelay(1.5f);
      sequence.Append(Icon.DOFade(0.1f, 0.05f).SetLoops(8, LoopType.Yoyo).SetEase(Ease.Linear));
      sequence.Append(Icon.DOFade(0, 0.07f).SetLoops(3, LoopType.Yoyo).SetEase(Ease.Linear).SetDelay(0.15f));
      sequence.Append(Icon.DOFade(1, 0.4f).SetEase(Ease.OutBounce));
      sequence.Append(Icon.DOFade(1, 0).SetDelay(1.5f));
      sequence.SetLoops(-1);
    }

    private void AppearAnimation()
    {    
      gameObject.SetActive(true);

      Background.alpha = 0;
      BodyTransform.localScale = Vector3.zero;
      BodyCanvasGroup.alpha = 1;
      
      Sequence sequence = DOTween.Sequence();
      sequence.Append(Background.DOFade(1, Duration));
      sequence.Join(BodyTransform.DOScale(.9f, Duration));
      sequence.Join(BodyCanvasGroup.DOFade(1, Duration));
    }  
    
    private void DisappearAnimation(Action onClose)
    {
      Sequence sequence = DOTween.Sequence();
      sequence.Append(Background.DOFade(0, Duration));
      sequence.Join(BodyCanvasGroup.DOFade(0, Duration));

      sequence.onComplete += () =>
      {
        gameObject.SetActive(false);
        onClose?.Invoke();
      };
    }
  }
}