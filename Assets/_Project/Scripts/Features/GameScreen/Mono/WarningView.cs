using System;
using _Project.Scripts.Animation;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio.Enum;
using _Project.Scripts.Services.Audio.Executor;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.GameScreen.Mono
{
  public class WarningView : MonoBehaviour
  {
    private const float Duration = .25f;
    [SerializeField] private Button YesButton;
    [SerializeField] private Button NoButton;

    private CanvasGroup _canvasGroup;

    private IAudioService _audioService;
    private IInputResolver _inputResolver;

    public void Construct(IInputResolver inputResolver, IAudioService audioService, Action onClickYes)
    {
      _audioService = audioService;
      _inputResolver = inputResolver;

      _canvasGroup = GetComponent<CanvasGroup>();

      YesButton.onClick.AddListener(() => onClickYes?.Invoke());
      NoButton.onClick.AddListener(() => SetActivity(false));

      new ButtonAnimation(YesButton.transform);
      new ButtonAnimation(NoButton.transform);

      new ButtonSound(_audioService, YesButton.gameObject, AudioId.Click);
      new ButtonSound(_audioService, NoButton.gameObject, AudioId.Click);
    }

    public void SetActivity(bool isOn)
    {
      _canvasGroup.DOFade(isOn ? 1f : 0f, Duration);
      _canvasGroup.blocksRaycasts = isOn;
      
      if (isOn)
        _inputResolver.Disable();
      else
        _inputResolver.Enable();
    }
  }
}