using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.GameScreen.Mono
{
  public class SymbolOperatorUI : MonoBehaviour
  {
    private const float Duration = .25f;
    
    private Image _icon;
    private CanvasGroup _canvasGroup;

    public void Construct(Sprite icon)
    {
      _icon = GetComponentInChildren<Image>();
      _canvasGroup = GetComponent<CanvasGroup>();

      _icon.sprite = icon;
    }

    public void SetActivity(bool isOn)
    {
      _canvasGroup.DOFade(isOn ? 1f : .3f, Duration);
    }
  }
}