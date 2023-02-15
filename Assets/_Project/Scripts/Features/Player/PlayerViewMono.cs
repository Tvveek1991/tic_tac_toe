using _Project.Scripts.Features.Games;
using _Project.Scripts.Features.GameScreen;
using _Project.Scripts.Features.GameScreen.Serialize;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Network.Mono;
using UnityEngine;
using Unity.Netcode;

namespace _Project.Scripts.Features.Player
{
  public class PlayerViewMono : NetworkBehaviour
  {
    private readonly NetworkVariable<bool> _netIsPlayerTurn = new NetworkVariable<bool>();
    private readonly NetworkVariable<SymbolSign> _netSymbolSign = new NetworkVariable<SymbolSign>();

    private int _index;
    
    private Camera _camera;
    private BoxCollider2D _boxCollider;

    private IGameFactory _gameFactory;
    private IGameMetaProvider _gameMetaProvider;

    private void OnMouseDown()
    {
      if(!IsOwner || !_netIsPlayerTurn.Value)
        return;
      
      Ray ray = _camera.ScreenPointToRay(Application.isEditor ? Input.mousePosition : (Vector3)Input.touches[0].position);

      if (!Physics.Raycast(ray, out RaycastHit hit)) 
        return;
      
      var cell = hit.transform.gameObject.GetComponent<SymbolCell>();
      cell.SetSymbolServerRpc(_netSymbolSign.Value);

      // ChangeTurnStatusServerRpc();
    }

    public override void OnDestroy()
    {
      _netSymbolSign.OnValueChanged -= OnValueChanged;
      _netIsPlayerTurn.OnValueChanged -= OnValueChangedPlayerTurn;
    }

    private void Construct(IGameFactory gameFactory, IGameMetaProvider gameMetaProvider)
    {
      _camera = Camera.main;
      _boxCollider = GetComponent<BoxCollider2D>();
      _boxCollider.enabled = true;
      
      _gameFactory = gameFactory;
      _gameMetaProvider = gameMetaProvider;

      _gameMetaProvider.SetPlayerViewMono(this);

      _index = (int) OwnerClientId;
      CommitNetworkColorServerRpc(GetSymbol());
    }

    public override void OnNetworkSpawn()
    {
      _netSymbolSign.OnValueChanged += OnValueChanged;

      name = IsOwner ? "Player_" + OwnerClientId : "Second Player";
      Debug.Log(name + " " + OwnerClientId);

      if (IsOwner)
        Construct(ClientDataTransmitter.Instance.GetGameFactory(), ClientDataTransmitter.Instance.GetGameMetaProvider());
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerTurnServerRpc(bool isPlayerTurn)
    {
      Debug.Log(name + " " + OwnerClientId + " Is " + isPlayerTurn);
      _netIsPlayerTurn.Value = isPlayerTurn;

      /*if (!test)
      {
        test = true;
        _netIsPlayerTurn.OnValueChanged += OnValueChangedPlayerTurn;
      }*/
    }

    // private bool test;

    private void OnValueChanged(SymbolSign previous, SymbolSign next)
    {
      if (!IsOwner)
        return;
      
      _gameFactory.GameViewLogic.SetPlayerUi(next);
    }

    private void OnValueChangedPlayerTurn(bool previous, bool next)
    {
      if (!IsOwner)
        return;

      _gameFactory.GameViewLogic.ChangeTurnServerRpc();
    }

    [ServerRpc]
    private void CommitNetworkColorServerRpc(SymbolSign symbol) =>
      _netSymbolSign.Value = symbol;

    private SymbolSign GetSymbol() =>
      (SymbolSign) (_index + 1);

    [ServerRpc]
    private void ChangeTurnStatusServerRpc() =>
      _netIsPlayerTurn.Value = !_netIsPlayerTurn.Value;
  }
}