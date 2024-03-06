using _Project.Scripts.Core_Logic.Common;
using _Project.Scripts.Features.Settings;
using _Project.Scripts.Features.Settings.Mono;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Money.Mono;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Features
{
  public class UiFactory : GameObjectFactory, IUiFactory
  {
    private const string MoneyCounterPath = "Features/Other/MoneyCounter";
    private const string SettingsPath = "Features/Windows/SettingsView";
    private const string NoInternetWindowPath = "Features/Other/Not Loaded Window";

    private const string BaseCanvas = "Canvas";

    private readonly IAssetProvider _assetProvider;

    public SettingsView SettingsView { get; private set;}
    public NotLoaded.NotLoadedWindow NoInternetWindow { get; private set;}

    public UiFactory(IAssetProvider assetProvider) : base(assetProvider) => 
      _assetProvider = assetProvider;

    public MoneyCounter SpawnMoneyCounter()
    {
      Transform parent = GameObject.Find(BaseCanvas).transform;

      var window = _assetProvider.GetResource<MoneyCounter>(MoneyCounterPath);
      return Object.Instantiate(window, parent);
    }
    
    public void CreateSettingsView()
    {
      Transform parent = GameObject.Find(BaseCanvas).transform;
      
      var window = _assetProvider.GetResource<SettingsView>(SettingsPath);
      SettingsView = Object.Instantiate(window, parent);
    }

    public void SpawnNoInternetConnection()
    {
      Transform parent = GameObject.Find(BaseCanvas).transform;

      var window = _assetProvider.GetResource<NotLoaded.NotLoadedWindow>(NoInternetWindowPath);
      NoInternetWindow = Object.Instantiate(window, parent);
    }

    public void ClearSettingsWindow()
    {
      if(SettingsView)
        Object.Destroy(SettingsView.gameObject);
    }
    

    public void ClearNoInternetWindow()
    {
      if(NoInternetWindow)
        Object.Destroy(NoInternetWindow.gameObject);
    }
  }
}