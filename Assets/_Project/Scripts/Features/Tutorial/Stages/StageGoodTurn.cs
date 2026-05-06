using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.Tutorial.Stages
{
  public class StageGoodTurn : StageTutorial
  {
    private const float Delay = .25f;
    private const float DurationToMovePoint = 1f;
    private const float DurationToArrowRotate = 1f;
    private const float Duration = .1f;

    [SerializeField] private RectTransform Arrow;
    [SerializeField] private RectTransform MovedRoll;
    
    [SerializeField] private Transform RollMovePoint;
    [SerializeField] private Transform RollMovePointCell;
    
    [SerializeField] private Image ArrowIcon;

    private readonly Vector2 _targetScale = new(188, 188);
    private readonly Vector3 _targetArrowRotate = new(0, 0, -20);

    private Sequence _sequence;

    public override void Construct()
    {
      _sequence = DOTween.Sequence();
      _sequence
        .SetLoops(-1, LoopType.Restart)
        .SetEase(Ease.Linear)
        .SetDelay(Delay)
        .Append(MovedRoll.DOSizeDelta(_targetScale, DurationToMovePoint))
        .Join(MovedRoll.DOMove(RollMovePoint.position, DurationToMovePoint))
        .Append(Arrow.DORotate(_targetArrowRotate, DurationToArrowRotate, RotateMode.FastBeyond360))
        .Append(ArrowIcon.DOFade(0f, Duration))
        .Join(MovedRoll.DOMove(RollMovePointCell.position, DurationToMovePoint));
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