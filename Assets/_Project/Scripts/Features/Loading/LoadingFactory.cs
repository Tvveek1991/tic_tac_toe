using System;
using _Project.Scripts.Core_Logic.Common;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.Loading
{
  public class LoadingFactory : GameObjectFactory, ILoadingFactory
  {
    private const string BaseCanvas = "Popup Canvas";
    private const string LoadingPath = "Features/Windows/Loading";
    
    public LoadingMono LoadingMono { get; private set; }

    public LoadingFactory(IAssetProvider assetProvider) : base(assetProvider)
    { }

    public void CreateScreen(Action onLoad)
    {
      ClearScreen();
      LoadingMono = Create<LoadingMono>(LoadingPath, GameObject.Find(BaseCanvas).transform);
      LoadingMono.Construct(onLoad, ClearScreen);
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