using _Project.Scripts.Core_Logic.Common;
using _Project.Scripts.Services._Interfaces;

namespace _Project.Scripts.Features.NotLoadedWindow
{
  public class NotLoadedFactory : GameObjectFactory, INotLoadedFactory
  {  
    private const string HomescreenPrefabPath = "Screens/Other/Not Loaded";

    public NotLoadedMono NotLoadedMono { get; private set; }

    public NotLoadedFactory(IAssetProvider assetProvider) : base(assetProvider)
    {}   
    
    public void CreateScreen()
    {
      ClearScreen();
      
      NotLoadedMono = Create<NotLoadedMono>(HomescreenPrefabPath);
      NotLoadedMono.gameObject.SetActive(false);
    }

    public void ClearScreen()
    {
      if (NotLoadedMono != null)
      {
        Clear(NotLoadedMono.gameObject);
        NotLoadedMono = null;
      }
    }
  }
}