using _Project.Scripts.Core_Logic.Common;
using _Project.Scripts.Features.Field;
using _Project.Scripts.Features.Field.Mono;
using _Project.Scripts.Features.GameScreen.Mono;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Network;
using UnityEngine;

namespace _Project.Scripts.Features.GameScreen
{
  public class GameFactory : GameObjectFactory, IGameFactory
  {
    private const string CanvasPath = "Canvas";
    
    private const string PlayerPath = "Features/Player/Player";
    private const string GameViewPath = "Features/GameScreen/Game Window";
    private const string GameFieldPath = "Features/GameScreen/Fields/Game Field";
    
    private GameViewMono GameViewMono { get; set; }
    private GameField GameField { get; set; }
    
    public IGameFieldLogic GameFieldLogic { get; private set; }
    public IGameViewLogic GameViewLogic { get; private set; }

    private readonly INetwork _network;
    private readonly ILocaleSystem _localeSystem;
    private readonly IAudioService _audioService;
    private readonly IGameMetaProvider _gameMetaProvider;

    public GameFactory(IAssetProvider assetProvider, ILocaleSystem localeSystem, IAudioService audioService, INetwork network,
      IGameMetaProvider gameMetaProvider) : base(assetProvider)
    {
      _network = network;
      _localeSystem = localeSystem;
      _audioService = audioService;
      _gameMetaProvider = gameMetaProvider;
    }
    
    public void CreateScreen()
    {
      GameViewMono = Create<GameViewMono>(GameViewPath, GameObject.Find(CanvasPath).transform);
      GameViewLogic = new GameViewLogic(GameViewMono, _localeSystem, _network, _gameMetaProvider);
    }
    
    public void CreateField()
    {
      GameField = Create<GameField>(GameFieldPath);
      GameFieldLogic = new GameFieldLogic(GameField, _audioService, _gameMetaProvider);
    }
    
    public void SpawnPlayerObject()
    {
      var playerViewMono = Create<PlayerViewMono>(PlayerPath);
      _gameMetaProvider.SetPlayerViewMono(playerViewMono);
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