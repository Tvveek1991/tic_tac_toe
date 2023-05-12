using System;
using _Project.Scripts.Services.Localization.Logic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.ScrollView
{
  public class ScrollViewSectionButton : MonoBehaviour
  {
    public Image Icon;
    public TextMeshProUGUI Description;

    public Button Button;

    public GameObject Selector;

    private Sprite _iconOn;
    private Sprite _iconOff;

    private string _nameKey;

    public void Construct(ILocaleSystem localeSystem, string nameKey, Action onClickOpenShop, Sprite iconOn = null, Sprite iconOff = null)
    {
      _nameKey = nameKey;

      if (iconOn)
      {
        Icon.sprite = iconOn;
        Icon.SetNativeSize();
        _iconOn = iconOn;
      }

      if (iconOff)
        _iconOff = iconOff;

      SetLocale(localeSystem);
      Button.onClick.AddListener(() => onClickOpenShop?.Invoke());
    }

    public void SetPosition(Vector3 position) =>
      transform.localPosition = position;
    
    public void Select(bool value)
    {
      Selector?.SetActive(value);
      Selector.GetComponent<Image>().DOFade(value ? 1 : 0, 0.35f);

      if (_iconOn && _iconOff)
        Icon.sprite = value ? _iconOn : _iconOff; 
    }

    private void SetLocale(ILocaleSystem localeSystem)
    {
      if (!Description)
        return;

      Description.text = localeSystem.GetText(_nameKey);
    }
  }
}