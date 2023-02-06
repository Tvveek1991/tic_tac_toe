using _Project.Scripts.Core_Logic.Common;
using _Project.Scripts.Features.Games.Classic;
using _Project.Scripts.Features.Games.Classic.Mono;
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
    private const string ClassicFieldPath = "Features/GameScreen/Fields/Classic Field";
    
    private GameViewMono GameViewMono { get; set; }
    private GameField GameField { get; set; }
    public PlayerViewMono PlayerViewMono { get; set; }
    
    public IClassicFieldLogic BaseFieldLogic { get; private set; }
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
      GameViewLogic = new GameViewLogic(GameViewMono, _localeSystem, _network);
    }
    
    public void CreateField()
    {
      GameField = Create<GameField>(ClassicFieldPath);
      BaseFieldLogic = new ClassicFieldLogic(GameField, _audioService);
    }
    
    public void SpawnPlayerObject()
    {
      PlayerViewMono = Create<PlayerViewMono>(PlayerPath);
      PlayerViewMono.Construct(_gameMetaProvider);
      
      _network.AddPlayer(PlayerViewMono);
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
      if (PlayerViewMono != null)
      {
        Clear(PlayerViewMono.gameObject);
        PlayerViewMono = null;
      }
    }

  }
}