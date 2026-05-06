using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.DisableAds.Mono
{
  public class SpriteSwap : MonoBehaviour
  {
    [SerializeField] private Image _icon;
    [SerializeField] private Sprite _crossIcon;
    [SerializeField] private Sprite _arrowIcon;

    public void SetIcon(bool isFirstOpen)
    {
      _icon.sprite = isFirstOpen ? _crossIcon : _arrowIcon;
      _icon.SetNativeSize();
    }
  }
}