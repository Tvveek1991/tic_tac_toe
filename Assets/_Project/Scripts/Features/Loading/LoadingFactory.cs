using System;
using _Project.Scripts.Core_Logic.Common;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using UnityEngine;

namespace _Project.Scripts.Features.Loading
{
  public class LoadingFactory : GameObjectFactory, ILoadingFactory
  {
    private const string BaseCanvas = "Popup Canvas";
    private const string LoadingPrefabPath = "Features/Windows/Loading";

    private readonly ILocaleSystem _localeSystem;
    private readonly IGameMetaProvider _gameMetaProvider;
    
    public LoadingMono LoadingMono { get; private set; }

    public LoadingFactory(IAssetProvider assetProvider, IGameMetaProvider gameMetaProvider, ILocaleSystem localeSystem) : base(assetProvider)
    {
      _localeSystem = localeSystem;
      _gameMetaProvider = gameMetaProvider;
    }

    public void CreateScreen(Action onEnd)
    {
      ClearScreen();
      LoadingMono = Create<LoadingMono>(LoadingPrefabPath, GameObject.Find(BaseCanvas).transform);
      LoadingMono.Construct(_gameMetaProvider, _localeSystem, onEnd);
    }

    public void ClearScreen()
    {
      if (LoadingMono != null)
      {
        Clear(LoadingMono.gameObject);
        LoadingMono = null;
      }
    }
  }
}