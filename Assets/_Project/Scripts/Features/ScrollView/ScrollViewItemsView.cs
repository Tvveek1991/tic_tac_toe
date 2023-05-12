using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Features.ScrollView
{
  public class ScrollViewItemsView : MonoBehaviour
  {
    private const float Duration = .3f;

    public Transform ContentParent;
    public CanvasGroup CanvasGroup;

    public void Activate()
    {
      CanvasGroup.DOFade(1f, Duration);
      CanvasGroup.blocksRaycasts = true;
    }

    public void Deactivate()
    {
      CanvasGroup.DOFade(0f, Duration);
      CanvasGroup.blocksRaycasts = false;
    }
  }
}