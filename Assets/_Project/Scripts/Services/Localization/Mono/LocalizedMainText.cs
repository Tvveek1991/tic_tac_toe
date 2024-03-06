using _Project.Scripts.Services.Localization.Logic;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Services.Localization.Mono
{
  [RequireComponent(typeof(TextMeshProUGUI))]
  public class LocalizedMainText : MonoBehaviour
  {
    [SerializeField] private string key;

    private TextMeshProUGUI _textComponent;
    private ILocaleSystem _localeSystem;

    public void Awake()
    {
      _textComponent = GetComponent<TextMeshProUGUI>();
      _textComponent.text = _localeSystem.GetText(key);
    }

    [Inject]
    private void Construct(ILocaleSystem localeSystem) => 
      _localeSystem = localeSystem;
  }
}