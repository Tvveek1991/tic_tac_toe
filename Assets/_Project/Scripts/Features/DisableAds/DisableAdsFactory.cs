using _Project.Scripts.Core_Logic.Common;
using _Project.Scripts.Features.DisableAds.Mono;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Features.DisableAds
{
  public class DisableAdsFactory : GameObjectFactory, IDisableAdsFactory
  {
    private const string DisableAdsPath = "Features/Windows/DisableAds";

    private const string BaseCanvas = "Canvas";

    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;
    private readonly IGameSaveSystem _gameSaveSystem;
    
    private DisableAdsWindow _disableAdsWindow;
    private DisableAdsViewLogic _disableAdsViewLogic;

    public DisableAdsFactory(IAssetProvider assetProvider, ILocaleSystem localeSystem, IAudioService audioService, IGameSaveSystem gameSaveSystem) : base(assetProvider)
    {
      _localeSystem = localeSystem;
      _audioService = audioService;
      _gameSaveSystem = gameSaveSystem;
    }

    public IDisableAdsViewLogic SpawnDisableAdsWindow()
    {
      Transform parent = GameObject.Find(BaseCanvas).transform;

      _disableAdsWindow = Create<DisableAdsWindow>(DisableAdsPath, parent);
      _disableAdsViewLogic = new DisableAdsViewLogic(_disableAdsWindow, _localeSystem, _audioService, _gameSaveSystem);

      return _disableAdsViewLogic;
    }

    public void Clear()
    {
      if (_disableAdsWindow == null)
        return;

      Object.Destroy(_disableAdsWindow.gameObject);
      _disableAdsWindow = null;
    }
  }
}