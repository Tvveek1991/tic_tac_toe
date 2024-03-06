using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.Services.Consent
{
  [RequireComponent(typeof(Button))]
  public class ConsentButton : MonoBehaviour
  {
    private ICmpManager _cmpManager;

    [Inject]
    public void Construct(ICmpManager cmpManager) => 
      _cmpManager = cmpManager;

    private void Start()
    {
      if (_cmpManager.IsFormAvailable())
        GetComponent<Button>().onClick.AddListener(Show);
      else
        transform.parent.gameObject.SetActive(false);
    }

    private void Show()
    {
      _cmpManager.ResetConsent();
      _cmpManager.Request(() => {Debug.Log("[Consent] Applied");});
    }
  }
}