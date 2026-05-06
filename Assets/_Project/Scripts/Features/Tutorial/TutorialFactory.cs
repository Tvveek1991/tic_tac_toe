using _Project.Scripts.Core_Logic.Common;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.Tutorial
{
  public class TutorialFactory : GameObjectFactory
  {
    private const string TutorialPath = "Features/Tutorial/Tutorial";
    private const string BaseCanvas = "Popup Canvas";
    
    private readonly IAssetProvider _assetProvider;

    public TutorialView TutorialView { get; private set; }
    
    public TutorialFactory(IAssetProvider assetProvider) : base(assetProvider)
    { }

    public void CreateTutorial()
    {
      Transform parent = GameObject.Find(BaseCanvas).transform;
      TutorialView = Create<TutorialView>(TutorialPath, parent);
    }

    public void Clear()
    {
      if (TutorialView == null)
        return;

      Object.Destroy(TutorialView.gameObject);
      TutorialView = null;
    }
  }
}