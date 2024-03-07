using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;

namespace _Project.Scripts.Features.NotLoaded
{
  public class NotLoadedWindowLogic : INotLoadedWindowLogic
  {
    private readonly INotLoadedWindowFactory _notLoadedWindowFactory;
    
    private NotLoadedWindow _notLoadedWindow;

    public NotLoadedWindowLogic(IAssetProvider assetProvider) => 
      _notLoadedWindowFactory = new NotLoadedWindowFactory(assetProvider);

    public void Open()
    {
      _notLoadedWindow = _notLoadedWindowFactory.SpawnNotLoadedWindow();
      _notLoadedWindow.Construct(Close);
      _notLoadedWindow.Open();
    }

    private void Close() => 
      _notLoadedWindow.Close(_notLoadedWindowFactory.Clear);
  }
}