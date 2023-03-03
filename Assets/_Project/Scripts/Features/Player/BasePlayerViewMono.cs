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
    protected IGameMetaProvider GameMetaProvider;

    public override void OnDestroy() =>
      NetSymbolSign.OnValueChanged -= OnValueChanged;

    public void Construct(IGameFactory gameFactory, IGameMetaProvider gameMetaProvider)
    {
      Camera = Camera.main;
      BoxCollider = GetComponent<BoxCollider2D>();
      BoxCollider.enabled = true;

      GameFactory = gameFactory;
      GameMetaProvider = gameMetaProvider;

      GameMetaProvider.SetPlayerViewMono(this);
    }

    public override void OnNetworkSpawn()
    {
      NetSymbolSign.OnValueChanged += OnValueChanged;

      name = IsOwner ? "Owner_" + OwnerClientId : "Second Player";
      Debug.Log(name + " " + OwnerClientId);

      if (!IsOwner) return;
      
      Construct(ClientDataTransmitter.Instance.GetGameFactory(), ClientDataTransmitter.Instance.GetGameMetaProvider());
      CreateSymbolSign((int) OwnerClientId);
    }

    public void CreateSymbolSign(int index)
    {
      Index = index;
      SetSymbolSignServerRpc(GetSymbol());
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerTurnServerRpc(bool isPlayerTurn)
    {
      Debug.Log(name + " Is " + isPlayerTurn);
      NetIsPlayerTurn.Value = isPlayerTurn;
    }

    private void OnValueChanged(SymbolSign previous, SymbolSign next)
    {
      if (!IsOwner)
        return;

      GameFactory.GameViewLogic.SetPlayerUi(next);
    }

    [ServerRpc]
    private void SetSymbolSignServerRpc(SymbolSign symbol) =>
      NetSymbolSign.Value = symbol;

    private SymbolSign GetSymbol() =>
      (SymbolSign) (Index + 1);
  }
}