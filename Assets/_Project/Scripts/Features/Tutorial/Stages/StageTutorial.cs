using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Features.Tutorial.Stages
{
  public class StageTutorial : MonoBehaviour
  {
    [SerializeField] protected CanvasGroup CanvasGroup;
    
    public virtual void Construct() {}
    
    public virtual void Show() => 
      CanvasGroup.DOFade(1f, .25f);

    public virtual void Hide() => 
      CanvasGroup.DOFade(0f, .25f);
  }
}