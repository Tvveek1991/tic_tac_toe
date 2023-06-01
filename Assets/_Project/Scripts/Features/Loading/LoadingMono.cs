using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Features.Loading
{
  public class LoadingMono : MonoBehaviour
  {
    public CanvasGroup CanvasGroup;
    public Transform Icon;

    private void Start() => 
      Animate();

    private void Animate()
    {
      CanvasGroup.alpha = 0;
      
      Icon.DORotate(new Vector3(0, 0, 360), 1f)
        .SetRelative(true)
        .SetLoops(-1, LoopType.Restart);

      CanvasGroup.DOFade(1, 0.3f).SetDelay(0.2f);
    }
  }
}