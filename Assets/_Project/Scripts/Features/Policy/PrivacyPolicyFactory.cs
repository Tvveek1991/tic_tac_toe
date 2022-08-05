using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.Policy
{
  public class PrivacyPolicyFactory : IPrivacyPolicyFactory
  {
    private const string CanvasBlurred = "Canvas";

    private const string UIPrivacyPolicyPath = "Features/Privacy/Privacy Policy Window";

    private PrivacyPolicy _privacyPolicy;

    private readonly IAssetProvider _assetProvider;

    public PrivacyPolicyFactory(IAssetProvider assetProvider)
    {
      _assetProvider = assetProvider;
    }

    public PrivacyPolicy SpawnPrivacyPolicy()
    {
      if (_privacyPolicy != null)
        return _privacyPolicy;

      Clear();

      Transform parent = GameObject.Find(CanvasBlurred).transform;

      var privacyPolicyObject = _assetProvider.GetResource<GameObject>(UIPrivacyPolicyPath);
      _privacyPolicy = Object.Instantiate(privacyPolicyObject, parent).GetComponent<PrivacyPolicy>();
      _privacyPolicy.gameObject.SetActive(false);
      return _privacyPolicy;
    }

    public void Clear()
    {
      if (!_privacyPolicy)
        return;

      _privacyPolicy.Clear();
      Object.Destroy(_privacyPolicy.gameObject);
    }
  }
}