using _Project.Scripts.Features.GameScreen;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Features.Player
{
  public class BasePlayerViewMono : MonoBehaviour
  {
    protected bool IsPlayerTurn;
    protected SymbolSign SymbolSign;
    
    protected Camera Camera;
    protected BoxCollider2D BoxCollider;

    protected IGameFactory GameFactory;
    protected IInputResolver InputResolver;
    protected IGameMetaProvider GameMetaProvider;

    public void Construct(IGameFactory gameFactory, IGameMetaProvider gameMetaProvider, IInputResolver inputResolver)
    {
      Camera = Camera.main;
      BoxCollider = GetComponent<BoxCollider2D>();
      BoxCollider.enabled = true;

      GameFactory = gameFactory;
      InputResolver = inputResolver;
      GameMetaProvider = gameMetaProvider;

      GameMetaProvider.SetPlayerViewMono(this);
      
      SymbolSign = GameMetaProvider.GetEnemyType() == EnemyType.OneDevice ? SymbolSign.X : (SymbolSign) Random.Range(1, 3);
    }

    public void SetPlayerTurn(bool isPlayerTurn) =>
      IsPlayerTurn = isPlayerTurn;

    public bool GetPlayerTurn() =>
      IsPlayerTurn;

    public SymbolSign GetSymbol() =>
      SymbolSign;

    public void ExchangeSymbolSign() => 
      SymbolSign = SymbolSign == SymbolSign.X ? SymbolSign.O : SymbolSign.X;
  }
}