using System;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.Loading
{
  public class LoadingLogic : ILoadingLogic
  {
    private readonly ILoadingFactory _loadingFactory;

    public LoadingLogic(IAssetProvider assetProvider) => 
      _loadingFactory = new LoadingFactory(assetProvider);

    public void Enter(Action onEnd) => 
      _loadingFactory.CreateScreen(() =>
      {
        onEnd?.Invoke();
        Debug.Log("Test");
      });

    public void Exit() => 
      _loadingFactory.ClearScreen();
  }
}