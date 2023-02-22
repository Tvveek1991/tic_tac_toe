using System;
using _Project.Scripts.Features.GameScreen.Mono;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Network;
using Unity.Netcode;

namespace _Project.Scripts.Features.GameScreen
{
  public class MultiplayerGameViewLogic : GameViewLogic
  {
    public MultiplayerGameViewLogic(GameViewMono gameViewMono, ILocaleSystem localeSystem, INetwork network, IGameMetaProvider gameMetaProvider, IAudioService audioService)
      : base(gameViewMono, localeSystem, network, gameMetaProvider, audioService)
    {
    }

    public override void Construct(Action onBack, Action onCreateNonConnectionPlayer)
    {
      base.Construct(onBack, onCreateNonConnectionPlayer);

      GameViewMono.Construct(ConnectClientObjectServerRpc, ConnectPlayersToNetworkServerRpc, RefreshAfterChangeTurnServerRpc);
    }

    [ServerRpc(RequireOwnership = false)]
    public override void ChangeTurningPlayerServerRpc() =>
      GameViewMono.ChangeTurningPlayerServerRpc();

    [ServerRpc]
    private void ConnectClientObjectServerRpc()
    {
      GameViewMono.SetLocalIpAddress(Network.GetLocalIPAddress());

      // Debug.Log("Start Spawn Player");

      OnCreateNonConnectionPlayer?.Invoke();
      SpawnPool.Add(GameMetaProvider.GetPlayerViewMono());
    }

    [ServerRpc]
    private void ConnectPlayersToNetworkServerRpc()
    {
      // Debug.Log("Client Count: " + _network.NetworkManager.ConnectedClients.Count);
      for (var i = 0; i < Network.NetworkManager.ConnectedClients.Count; i++)
      {
        SpawnPool[i].NetworkObject.SpawnAsPlayerObject((uint) i, true);
        // Debug.Log("SpawnedPlayer + " + i);
      }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RefreshAfterChangeTurnServerRpc(int activePlayer)
    {
      GameMetaProvider.GetPlayerViewMono().SetPlayerTurnServerRpc(GameMetaProvider.GetPlayerViewMono().OwnerClientId == (ulong) activePlayer);
      GameViewMono.TurnText.text = GameMetaProvider.GetPlayerViewMono().OwnerClientId == (ulong) activePlayer ? "Your turn" : "Enemy turn";
    }
  }
}