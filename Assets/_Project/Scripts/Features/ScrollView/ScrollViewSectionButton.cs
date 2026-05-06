using System;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio.Enum;
using _Project.Scripts.Services.Audio.Executor;
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
    public TextMeshProUGUI Name;

    public Button Button;

    public Image Selector;

    private Sprite _iconOn;
    private Sprite _iconOff;

    private string _nameKey;

    public void Construct(IAudioService audioService, ILocaleSystem localeSystem, string nameKey, Action onClickOpenShop,
      Sprite iconOn = null, Sprite iconOff = null)
    {
      _nameKey = nameKey;

      /*if (iconOn)
      {
        Icon.sprite = iconOn;
        Icon.SetNativeSize();
        _iconOn = iconOn;
      }

      if (iconOff)
        _iconOff = iconOff;*/

      SetLocale(localeSystem);
      
      Button.onClick.AddListener(() => onClickOpenShop?.Invoke());

      new ButtonSound(audioService, Button.gameObject, AudioId.Click);
    }

    public void SetPosition(Vector3 position) =>
      transform.localPosition = position;
    
    public void Select(bool value)
    {
      // Selector.SetActive(value);
      Selector.DOFade(value ? 1 : 0.05f, 0.35f);

      /*if (_iconOn && _iconOff)
        Icon.sprite = value ? _iconOn : _iconOff; */
    }

    private void SetLocale(ILocaleSystem localeSystem)
    {
      if (!Name)
        return;

      Name.text = localeSystem.GetText(_nameKey);
    }
  }
}