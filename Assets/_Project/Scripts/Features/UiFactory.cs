using _Project.Scripts.Core_Logic.Common;
using _Project.Scripts.Features.NotLoaded;
using _Project.Scripts.Features.Settings.Mono;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Features
{
  public class UiFactory : GameObjectFactory, IUiFactory
  {
    private const string SettingsPath = "Features/Windows/SettingsView";
    private const string NoInternetWindowPath = "Features/Other/Not Loaded View";

    private const string BaseCanvas = "Canvas";

    private readonly IAssetProvider _assetProvider;

    public SettingsView SettingsView { get; private set;}
    public NotLoadedWindow NoInternetView { get; private set;}

    public UiFactory(IAssetProvider assetProvider) : base(assetProvider) => 
      _assetProvider = assetProvider;

    public void CreateSettingsView()
    {
      Transform parent = GameObject.Find(BaseCanvas).transform;
      
      var window = Create<SettingsView>(SettingsPath);
      SettingsView = Object.Instantiate(window, parent);
    }

    public void SpawnNoInternetConnectionView()
    {
      Transform parent = GameObject.Find(BaseCanvas).transform;

      var window = Create<NotLoadedWindow>(NoInternetWindowPath);
      NoInternetView = Object.Instantiate(window, parent);
    }

    public void ClearSettingsView()
    {
      if(SettingsView)
        Object.Destroy(SettingsView.gameObject);
    }
    
    public void ClearNoInternetView()
    {
      if(NoInternetView)
        Object.Destroy(NoInternetView.gameObject);
    }
  }
}