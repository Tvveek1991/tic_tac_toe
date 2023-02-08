using _Project.Scripts.Features.GameScreen;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Services.Network.Mono
{
  public class ClientDataTransmitter : MonoBehaviour
  {
    public static ClientDataTransmitter Instance;
    
    private IGameFactory _gameFactory;
    private IGameMetaProvider _gameMetaProvider;

    private void Awake()
    {
      if (Instance == null)
      {
        Instance = this;
        DontDestroyOnLoad(gameObject);
      }
      else
      {
        Destroy(gameObject);
      }
    }
    
    public void Construct(IGameFactory gameFactory, IGameMetaProvider gameMetaProvider)
    {
      _gameFactory = gameFactory;
      _gameMetaProvider = gameMetaProvider;
    }

    public IGameFactory GetGameFactory() =>
      _gameFactory;
    
    public IGameMetaProvider GetGameMetaProvider() =>
      _gameMetaProvider;
  }
}