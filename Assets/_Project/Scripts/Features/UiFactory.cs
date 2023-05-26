using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Money.Mono;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Features
{
  public class UiFactory : IUiFactory
  {
    private const string MoneyCounterPath = "Features/Other/MoneyCounter";
    private const string SettingsPath = "Features/Windows/SettingsWindow";
    private const string LoadWindowPath = "Features/Windows/LoadWindow";
    private const string NoInternetWindowPath = "Features/Other/Not Loaded Window";

    private const string BaseCanvas = "Canvas";

    private readonly IAssetProvider _assetProvider;

    // public LoadWindow Loading { get; private set; }
    public Settings.Settings SettingsWindow { get; private set;}
    public NotLoaded.NotLoadedWindow NoInternetWindow { get; private set;}

    public UiFactory(IAssetProvider assetProvider)
    {
      _assetProvider = assetProvider;
    }

    /*public LoadWindow CreateLoadWindow()
    {
      ClearLoadWindow();

      Transform parent = GameObject.Find(BaseCanvas).transform;

      var loseScreen = _assetProvider.GetResource<LoadWindow>(LoadWindowPath);
      Loading = Object.Instantiate(loseScreen, parent);
      
      return Loading;
    }*/

    public MoneyCounter SpawnMoneyCounter()
    {
      Transform parent = GameObject.Find(BaseCanvas).transform;

      var window = _assetProvider.GetResource<MoneyCounter>(MoneyCounterPath);
      return Object.Instantiate(window, parent);
    }
    
    public void SpawnSettingsWindow()
    {
      Transform parent = GameObject.Find(BaseCanvas).transform;

      var window = _assetProvider.GetResource<Settings.Settings>(SettingsPath);
      SettingsWindow = Object.Instantiate(window, parent);
    }

    public void SpawnNoInternetConnection()
    {
      Transform parent = GameObject.Find(BaseCanvas).transform;

      var window = _assetProvider.GetResource<NotLoaded.NotLoadedWindow>(NoInternetWindowPath);
      NoInternetWindow = Object.Instantiate(window, parent);
    }

    public void ClearLoadWindow()
    {
      /*if(Loading)
        Object.Destroy(Loading.gameObject);*/
    }
    
    public void ClearSettingsWindow()
    {
      SettingsWindow?.Clear();
      
      if(SettingsWindow)
        Object.Destroy(SettingsWindow.gameObject);
    }

    public void ClearNoInternetWindow()
    {
      /*if(NoInternetWindow)
        Object.Destroy(NoInternetWindow.gameObject);*/
    }
  }
}