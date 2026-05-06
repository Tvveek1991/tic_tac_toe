using System;
using System.Collections;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Features.GameScreen.Mono
{
  public class PreparingMono : MonoBehaviour
  {
    private const float Duration = .2f;
    private const float DurationArrow = .4f;
    private const float WaitingTime = .5f;

    [SerializeField] private TextMeshProUGUI _title;

    [SerializeField] private CanvasGroup CanvasGroup;
    [SerializeField] private CanvasGroup RollingFirstTurn;

    [SerializeField] private CanvasGroup Loading;
    [SerializeField] private Transform Arrow;

    [SerializeField] private Button StartButton;

    [SerializeField] private TextMeshProUGUI SecondPlayer;
    [SerializeField] private TextMeshProUGUI FirstPlayer;

    private Action _onEnd;

    private int _cycleCounter;

    private ILocaleSystem _localeSystem;
    private IGameMetaProvider _gameMetaProvider;

    public void Construct(IGameMetaProvider gameMetaProvider, ILocaleSystem localeSystem, Action onEnd)
    {
      _onEnd = onEnd;
      _localeSystem = localeSystem;
      _gameMetaProvider = gameMetaProvider;

      CanvasGroup.alpha = 1;
      CanvasGroup.blocksRaycasts = true;

      StartButton.onClick.AddListener(() =>
      {
        StartButton.interactable = false;
        StartCoroutine(WaitBotStart());
      });
    }

    private IEnumerator WaitBotStart()
    {
      _title.text = _localeSystem.GetText("WaitingOpponent");

      int rnd = IsBot() ? Random.Range(0, 4) : 0;

      if (IsBot())
      {
        Loading.DOFade(1f, Duration);
        Loading.transform.DORotate(Vector3.forward * -360, DurationArrow)
          .SetRelative(true)
          .SetEase(Ease.Linear)
          .SetLoops(-1, LoopType.Restart);
      }

      yield return new WaitForSecondsRealtime(rnd);
      Loading.DOFade(0f, Duration);

      if (IsBot())
        StartCoroutine(RollFirstTurn());
      else
        _onEnd?.Invoke();
    }

    private IEnumerator RollFirstTurn()
    {
      _title.text = _localeSystem.GetText("WhoIsFirst");

      _cycleCounter = Random.Range(5, 10);
      RollingFirstTurn.DOFade(1f, WaitingTime);

      yield return new WaitForSecondsRealtime(WaitingTime);

      AnimateArrow();

      while (_cycleCounter-- > 0)
      {
        Swap(true);
        yield return new WaitForSecondsRealtime(Duration);

        Swap(false);
        yield return new WaitForSecondsRealtime(Duration);
      }

      yield return new WaitForSecondsRealtime(IsPlayerSymbolX() ? 0 : Duration);
      Swap(IsPlayerSymbolX());

      CanvasGroup.DOFade(0, WaitingTime * 3).SetDelay(0.2f)
        .OnComplete(() =>
        {
          _onEnd?.Invoke();
          CanvasGroup.blocksRaycasts = false;
        });
    }

    private void AnimateArrow()
    {
      int endValue = (-360 * _cycleCounter) - (IsPlayerSymbolX() ? 320 : 140);
      Arrow.GetComponent<Image>().DOFade(1f, Duration);
      Arrow.DORotate(new Vector3(0, 0, endValue), DurationArrow * _cycleCounter)
        .SetRelative(true)
        .SetEase(Ease.Linear);
    }

    private void Swap(bool isFirst)
    {
      FirstPlayer.DOFade(isFirst ? 1f : 0.5f, Duration);
      FirstPlayer.transform.DOScale(isFirst ? 1.5f : 1f, Duration);
      SecondPlayer.DOFade(isFirst ? 0.5f : 1f, Duration);
      SecondPlayer.transform.DOScale(isFirst ? 1f : 1.5f, Duration);
    }

    private bool IsPlayerSymbolX() =>
      _gameMetaProvider.GetPlayerView().GetSymbol() == SymbolSign.X;

    private bool IsBot() =>
      _gameMetaProvider.GetEnemyType() == EnemyType.Bot;
  }
}