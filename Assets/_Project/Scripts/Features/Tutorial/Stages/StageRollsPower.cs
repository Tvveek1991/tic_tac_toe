using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Features.Tutorial.Stages
{
  public class StageRollsPower : StageTutorial
  {
    private Sequence _sequence;

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