using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.Tutorial.Stages
{
  public class StageBadTurn : StageTutorial
  {
    private const float Delay = .25f;
    private const float DurationToMovePoint = 1f;
    private const float DurationToCellPoint = .25f;

    [SerializeField] private RectTransform MovedRoll;

    [SerializeField] private CanvasGroup IconLockDots;

    [SerializeField] private Transform RollMovePoint;
    [SerializeField] private Transform RollMovePointCell;

    private readonly Vector2 _targetScale = new(1.88f, 1.88f);

    private Sequence _sequence;

    public override void Construct()
    {
      _sequence = DOTween.Sequence();
      _sequence
        .SetLoops(-1, LoopType.Restart)
        .SetEase(Ease.Linear)
        .SetDelay(Delay)
        .Append(MovedRoll.DOScale(_targetScale, DurationToMovePoint))
        .Join(MovedRoll.DOMove(RollMovePoint.position, DurationToMovePoint))
        .Append(MovedRoll.DOMove(RollMovePointCell.position, DurationToCellPoint).SetLoops(2, LoopType.Yoyo).OnStepComplete(() =>
          { IconLockDots.DOFade(1, .1f); }))
        .AppendInterval(1f)
        .OnStepComplete(() => { IconLockDots.alpha = 0; })
        .OnRewind(() => { IconLockDots.alpha = 0; });
    }

    public override void Show()
    {
      base.Show();
      _sequence.Play();
    }

    public override void Hide()
    {
      base.Hide();
      _sequence.Rewind();
    }
  }
}