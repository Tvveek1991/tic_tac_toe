using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.GameScreen;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Network.Mono;
using UnityEngine;
using Unity.Netcode;

namespace _Project.Scripts.Features.Player
{
  public class BasePlayerViewMono : NetworkBehaviour
  {
    protected readonly NetworkVariable<bool> NetIsPlayerTurn = new NetworkVariable<bool>();
    protected readonly NetworkVariable<SymbolSign> NetSymbolSign = new NetworkVariable<SymbolSign>();

    protected int Index;

    protected Camera Camera;
    protected BoxCollider2D BoxCollider;

    protected IGameFactory GameFactory;
    protected IInputResolver InputResolver;
    protected IGameMetaProvider GameMetaProvider;

    public override void OnDestroy() =>
      NetSymbolSign.OnValueChanged -= OnValueChanged;

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
      Index = index;
      
      if(GameMetaProvider.GetGameMode() == GameMode.Single)
        SetSymbolSign(GetSymbol());
      else
        SetSymbolSignServerRpc(GetSymbol());
    }

    public void SetPlayerTurn(bool isPlayerTurn)
    {
      Debug.Log(name + " Is " + isPlayerTurn);
      NetIsPlayerTurn.Value = isPlayerTurn;
    }

    private void SetSymbolSign(SymbolSign symbol) =>
      NetSymbolSign.Value = symbol;

    protected SymbolSign GetSymbol() =>
      (SymbolSign) (Index + 1);

    #region Multiplayer

    public override void OnNetworkSpawn()
    {
      NetSymbolSign.OnValueChanged += OnValueChanged;

      name = IsOwner ? "Owner_" + OwnerClientId : "Second Player";
      Debug.Log(name + " " + OwnerClientId);

      if (!IsOwner) return;

      Construct(ClientDataTransmitter.Instance.GetGameFactory(), ClientDataTransmitter.Instance.GetGameMetaProvider(), ClientDataTransmitter.Instance.GetInputResolver());
      CreateSymbolSign((int) OwnerClientId);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerTurnServerRpc(bool isPlayerTurn)
    {
      Debug.Log(name + " Is " + isPlayerTurn);
      NetIsPlayerTurn.Value = isPlayerTurn;
    }

    [ServerRpc]
    private void SetSymbolSignServerRpc(SymbolSign symbol) =>
      NetSymbolSign.Value = symbol;

    private void OnValueChanged(SymbolSign previous, SymbolSign next)
    {
      if (!IsOwner)
        return;

      GameFactory.GameViewLogic.SetPlayerUi(next);
    }

    #endregion
  }
}