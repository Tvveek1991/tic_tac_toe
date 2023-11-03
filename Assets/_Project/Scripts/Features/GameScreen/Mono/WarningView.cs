using System;
using _Project.Scripts.Animation;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio.Enum;
using _Project.Scripts.Services.Audio.Executor;
using _Project.Scripts.Services.Localization.Logic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.GameScreen.Mono
{
  public class WarningView : MonoBehaviour
  {
    private const float Duration = .3f;
    private const float MovingDuration = .4f;
    
    [SerializeField] private Button YesButton;
    [SerializeField] private Button NoButton;
    
    [SerializeField] private RectTransform _offer;

    private CanvasGroup _canvasGroup;

    private IAudioService _audioService;
    private IInputResolver _inputResolver;

    public void Construct(IInputResolver inputResolver, IAudioService audioService, Action onClickYes)
    {
      _audioService = audioService;
      _inputResolver = inputResolver;

      _canvasGroup = GetComponent<CanvasGroup>();

      YesButton.onClick.AddListener(() => onClickYes?.Invoke());
      NoButton.onClick.AddListener(Hide);

      new ButtonAnimation(YesButton.transform);
      new ButtonAnimation(NoButton.transform);

      new ButtonSound(_audioService, YesButton.gameObject, AudioId.Click);
      new ButtonSound(_audioService, NoButton.gameObject, AudioId.Click);

      _canvasGroup.alpha = 0;
    }

    public void Show()
    {
      _offer.DOLocalMoveY(120f, MovingDuration)
        .OnComplete(() =>  _canvasGroup.blocksRaycasts = true);
      
      _canvasGroup.DOFade(1f, Duration);
      _inputResolver.Disable();
    }

    private void Hide()
    {
      _canvasGroup.blocksRaycasts = false;
      _offer.DOLocalMoveY(900f, MovingDuration);
      _canvasGroup.DOFade(0f, Duration).SetDelay(Duration);
      _inputResolver.Enable();
    }
  }
}