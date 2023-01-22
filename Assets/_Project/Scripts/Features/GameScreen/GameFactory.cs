using _Project.Scripts.Core_Logic.Common;
using _Project.Scripts.Features.Games.Classic;
using _Project.Scripts.Features.Games.Classic.Mono;
using _Project.Scripts.Features.GameScreen.Mono;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using UnityEngine;

namespace _Project.Scripts.Features.GameScreen
{
  public class GameFactory : GameObjectFactory, IGameFactory
  {
    private const string CanvasPath = "Canvas";

    private const string GameViewPath = "Features/GameScreen/Game Window";
    
    private const string ClassicFieldPath = "Features/GameScreen/Fields/Classic Field";
    
    private GameViewMono GameViewMono { get; set; }
    private GameField GameField { get; set; }
    
    public IClassicFieldLogic BaseFieldLogic { get; private set; }
    public IGameViewLogic GameViewLogic { get; private set; }

    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;

    public GameFactory(IAssetProvider assetProvider, ILocaleSystem localeSystem, IAudioService audioService) : base(assetProvider)
    {
      _localeSystem = localeSystem;
      _audioService = audioService;
    }
    
    public void CreateScreen()
    {
      GameViewMono = Create<GameViewMono>(GameViewPath, GameObject.Find(CanvasPath).transform);
      GameViewLogic = new GameViewLogic(GameViewMono, _localeSystem);
    }
    
    public void CreateField()
    {
      GameField = Create<GameField>(ClassicFieldPath);
      BaseFieldLogic = new ClassicFieldLogic(GameField, _audioService);
    }
    
    public void ClearScreen()
    {
      if (GameViewMono != null)
      {
        Clear(GameViewMono.gameObject);
        GameViewMono = null;
      }
      
      if (GameField != null)
      {
        Clear(GameField.gameObject);
        GameField = null;
      }
    }

  }
}