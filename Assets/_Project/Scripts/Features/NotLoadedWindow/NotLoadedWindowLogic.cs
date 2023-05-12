using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;

namespace _Project.Scripts.Features.NotLoadedWindow
{
  public class NotLoadedWindowLogic : INotLoadedWindowLogic
  {
    private readonly INotLoadedWindowFactory _notLoadedWindowFactory;

    private readonly ILocaleSystem _localeSystem;
    
    private NotLoadedWindow _notLoadedWindow;

    public NotLoadedWindowLogic(ILocaleSystem localeSystem, IAssetProvider assetProvider)
    {
      _localeSystem = localeSystem;
      _notLoadedWindowFactory = new NotLoadedWindowFactory(assetProvider);
    }

    public void Open()
    {
      _notLoadedWindow = _notLoadedWindowFactory.SpawnNotLoadedWindow();
      _notLoadedWindow.Construct(_localeSystem, Close);
      _notLoadedWindow.Open();
    }

    private void Close() => 
      _notLoadedWindow.Close(_notLoadedWindowFactory.Clear);
  }
}