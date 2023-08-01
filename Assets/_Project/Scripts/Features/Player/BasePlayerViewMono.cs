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

    protected IGameFactory GameFactory;
    protected IInputResolver InputResolver;
    
    public void Construct(IGameFactory gameFactory, IGameMetaProvider gameMetaProvider, IInputResolver inputResolver)
    {
      Camera = Camera.main;

      GameFactory = gameFactory;
      InputResolver = inputResolver;
      
      SymbolSign = gameMetaProvider.GetEnemyType() == EnemyType.OneDevice ? SymbolSign.X : (SymbolSign) Random.Range(1, 3);
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