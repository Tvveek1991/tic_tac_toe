using System;
using System.Collections.Generic;
using _Project.Scripts.Features.GameScreen.Mono;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Network;
using Unity.Netcode;

namespace _Project.Scripts.Features.GameScreen
{
  public class MultiplayerGameViewLogic : GameViewLogic
  {
    private readonly List<BasePlayerViewMono> _spawnPool = new List<BasePlayerViewMono>();
    
    private readonly INetwork _network;
    
    public MultiplayerGameViewLogic(GameViewMono gameViewMono, ILocaleSystem localeSystem, INetwork network, IGameMetaProvider gameMetaProvider, IAudioService audioService)
      : base(gameViewMono, localeSystem, gameMetaProvider, audioService)
    {
      _network = network;
    }

    public override void Construct(Action onBack, Action onCreateNonConnectionPlayer)
    {
      base.Construct(onBack, onCreateNonConnectionPlayer);

      GameViewMono.Construct(ConnectClientObjectServerRpc, ConnectPlayersToNetworkServerRpc, RefreshAfterChangeTurnServerRpc);
    }

    public override void ChangeTurningPlayer(SymbolSign activeSign) =>
      GameViewMono.ChangeTurningPlayerServerRpc();

    [ServerRpc]
    private void ConnectClientObjectServerRpc()
    {
      GameViewMono.SetLocalIpAddress(_network.GetLocalIPAddress());

      // Debug.Log("Start Spawn Player");

      OnCreateNonConnectionPlayer?.Invoke();
      _spawnPool.Add(GameMetaProvider.GetPlayerViewMono());
    }

    [ServerRpc]
    private void ConnectPlayersToNetworkServerRpc()
    {
      // Debug.Log("Client Count: " + _network.NetworkManager.ConnectedClients.Count);
      for (var i = 0; i < _network.NetworkManager.ConnectedClients.Count; i++)
      {
        _spawnPool[i].NetworkObject.SpawnAsPlayerObject((uint) i, true);
        // Debug.Log("SpawnedPlayer + " + i);
      }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RefreshAfterChangeTurnServerRpc(int activePlayer)
    {
      GameMetaProvider.GetPlayerViewMono().SetPlayerTurnServerRpc(GameMetaProvider.GetPlayerViewMono().OwnerClientId == (ulong) activePlayer);
      GameViewMono.TurnText.text = GameMetaProvider.GetPlayerViewMono().OwnerClientId == (ulong) activePlayer ? "Your turn" : "Enemy turn";

      if (GameMetaProvider.GetGame() != Game.Buhurt) 
        return;
      
      SymbolSign symbol = (SymbolSign) activePlayer + 1;
      GameViewMono.LoadBuhurtPart(_playerData[symbol], _figuresState[symbol]);
      GameViewMono.SetVisibleBuhurtPart(GameMetaProvider.GetPlayerViewMono().OwnerClientId == (uint)activePlayer);
    }
  }
}