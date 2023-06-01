using System;
using _Project.Scripts.Core_Logic.Common;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.Loading
{
  public class LoadingFactory : GameObjectFactory, ILoadingFactory
  {
    private const string BaseCanvas = "Canvas";
    private const string LoadingPrefabPath = "Features/Windows/Loading";

    public LoadingMono LoadingMono { get; private set; }

    public LoadingFactory(IAssetProvider assetProvider) : base(assetProvider)
    {}

    public void CreateScreen(Action onEnd)
    {
      ClearScreen();
      LoadingMono = Create<LoadingMono>(LoadingPrefabPath, GameObject.Find(BaseCanvas).transform);
      LoadingMono.Construct(onEnd);
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