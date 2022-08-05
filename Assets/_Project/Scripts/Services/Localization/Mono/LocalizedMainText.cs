using _Project.Scripts.Services.Localization.Logic;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Services.Localization.Mono
{
  [RequireComponent(typeof(TMP_Text))]
  public class LocalizedMainText : MonoBehaviour
  {
    [SerializeField] private TMP_Text textComponent;
    [SerializeField] private string key;

    private ILocaleSystem _localeSystem;

    public void Construct(ILocaleSystem localeSystem)
    {
      _localeSystem = localeSystem;
       Localize();
    }

    public void Localize()
    {
      if (_localeSystem != null && textComponent != null)
        textComponent.text = _localeSystem.GetText(key);
    }
    
    public void SetNewKey(string newKey)
    {
      key = newKey;
      Localize();
    }
  }
}