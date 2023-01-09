using _Project.Scripts.Features.Games.Classic.Mono;
using _Project.Scripts.Features.GameScreen.Mono;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.GameScreen
{
  public class GameFactory : IGameFactory
  {
    private const string BaseCanvas = "Canvas";
    
    private const string GameViewPath = "Features/GameScreen/Game Window";
    
    private readonly IAssetProvider _assetProvider;

    public GameFactory(IAssetProvider assetProvider)
    {
      _assetProvider = assetProvider;
    }

    public GameView CreatViewScreen()
    {
      Transform parent = GameObject.Find(BaseCanvas).transform;

      var gameView = _assetProvider.GetResource<GameView>(GameViewPath);
      return Object.Instantiate(gameView, parent);
    }
    
    public GameFieldView CreateField(string path)
    {
      var gameField = _assetProvider.GetResource<GameFieldView>(path);
      return Object.Instantiate(gameField);
    }
  }
}