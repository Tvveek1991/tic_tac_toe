using _Project.Scripts.Features.GameScreen;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.Player
{
  public class BasePlayerViewMono : MonoBehaviour
  {
    protected bool IsPlayerTurn;
    protected SymbolSign symbolSign;
    
    protected Camera Camera;
    protected BoxCollider2D BoxCollider;

    protected IGameFactory GameFactory;
    protected IInputResolver InputResolver;
    protected IGameMetaProvider GameMetaProvider;

    private int _index;

    public void Construct(IGameFactory gameFactory, IGameMetaProvider gameMetaProvider, IInputResolver inputResolver)
    {
      Camera = Camera.main;
      BoxCollider = GetComponent<BoxCollider2D>();
      BoxCollider.enabled = true;

      GameFactory = gameFactory;
      InputResolver = inputResolver;
      GameMetaProvider = gameMetaProvider;

      GameMetaProvider.SetPlayerViewMono(this);
    }

    public void CreateSymbolSign(int index)
    {
      _index = index;
      SetSymbolSign(GetSymbol());
    }

    public void SetPlayerTurn(bool isPlayerTurn)
    {
      // Debug.Log(name + " Is " + _isPlayerTurn);
      IsPlayerTurn = isPlayerTurn;
    }

    public bool GetPlayerTurn() =>
      IsPlayerTurn;

    private void SetSymbolSign(SymbolSign symbol) =>
      symbolSign = symbol;

    protected SymbolSign GetSymbol() =>
      (SymbolSign) (_index + 1);
  }
}