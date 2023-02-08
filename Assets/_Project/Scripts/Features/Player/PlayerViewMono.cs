using System;
using System.Collections.Generic;
using _Project.Scripts.Features.Games;
using _Project.Scripts.Features.GameScreen;
using _Project.Scripts.Features.GameScreen.Serialize;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.GameMetaProvider;
using _Project.Scripts.Services.Network.Mono;
using UnityEngine;
using Unity.Netcode;

namespace _Project.Scripts.Features.Player
{
  public class PlayerViewMono : NetworkBehaviour
  {
    private readonly Vector3 _spawnPosition = new Vector3(0, 10, 1);

    private readonly NetworkVariable<SymbolSign> _netSymbolSign = new NetworkVariable<SymbolSign>();

    private int _index;

    private IGameFactory _gameFactory;
    private IGameMetaProvider _gameMetaProvider;

    private void Update()
    {
      //ToDo Переделать связку с объектами
      if (!Input.GetMouseButtonDown(0) || !IsOwner)
        return;

      CheckCell();
    }

    public override void OnDestroy() =>
      _netSymbolSign.OnValueChanged -= OnValueChanged;

    private void Construct(IGameFactory gameFactory, IGameMetaProvider gameMetaProvider)
    {
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
      else
        enabled = false;
    }

    private void OnValueChanged(SymbolSign previous, SymbolSign next)
    {
      if (IsOwner)
        _gameFactory.GameViewLogic.SetPlayerUi(next);
    }

    private void CheckCell()
    {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;

      if (Physics.Raycast(ray, out hit))
      {
        GameObject hitObject = hit.transform.gameObject;
        hitObject.GetComponent<SymbolCell>().SetSymbolServerRpc(_netSymbolSign.Value);
      }
    }

    [ServerRpc]
    private void CommitNetworkColorServerRpc(SymbolSign symbol) =>
      _netSymbolSign.Value = symbol;

    private SymbolSign GetSymbol() =>
      (SymbolSign) (_index + 1);
  }
}