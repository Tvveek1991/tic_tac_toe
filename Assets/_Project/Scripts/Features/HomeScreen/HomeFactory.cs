using _Project.Scripts.Core_Logic.Common;
using _Project.Scripts.Features.HomeScreen.Mono;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using UnityEngine;

namespace _Project.Scripts.Features.HomeScreen
{
  public class HomeFactory : GameObjectFactory, IHomeFactory
  {
    private const string BaseCanvas = "Canvas";
    
    private const string HomeViewPath = "Features/HomeScreen/Home";

    private HomeViewMono HomeViewMono { get; set; }
    public IHomeViewLogic HomeViewLogic { get; private set; }

    private readonly IAudioService _audioService;
    private readonly ILocaleSystem _localeSystem;
    private readonly IGameMetaProvider _gameMetaProvider;
    
    public HomeFactory(IAssetProvider assetProvider, ILocaleSystem localeSystem, IAudioService audioService, IGameMetaProvider gameMetaProvider) 
      : base(assetProvider)
    {
      _audioService = audioService;
      _localeSystem = localeSystem;
      _gameMetaProvider = gameMetaProvider;
    }

    public void CreateHomeView()
    {
      ClearHomeView();
      HomeViewMono = Create<HomeViewMono>(HomeViewPath, GameObject.Find(BaseCanvas).transform);
      HomeViewLogic = new HomeViewLogic(HomeViewMono, _localeSystem, _audioService, _gameMetaProvider);
    }

    public void ClearHomeView()
    {
      if (!HomeViewMono) return;
      
      Object.Destroy(HomeViewMono.gameObject);
    }
  }
}