using System.Collections.Generic;
using _Project.Scripts.Core_Logic.Common;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Network.Mono;
using Unity.Netcode;
using UnityEngine;

namespace _Project.Scripts.Services.Network
{
  public class Network : GameObjectFactory, INetwork
  {
    private const string NetworkManagerPath = "Features/Network/Network Manager";
    private const string PlayerPath = "Features/Player/Player";

    public NetworkManager NetworkManager { get; private set; }
    public PlayerViewMono PlayerViewMono { get; private set; }

    public Network(IAssetProvider assetProvider) : base(assetProvider) { }

    public void LaunchService() => 
      SpawnNetworkManager();

    public void OnPlayerSpawn(bool isHost)
    {
      if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer) 
        return;
      
      if (isHost)
        NetworkManager.StartHost();
      else
        NetworkManager.StartClient();
    }

    public void SpawnPlayers()
    {
      foreach (KeyValuePair<ulong, NetworkClient> item in NetworkManager.ConnectedClients)
      {
        PlayerViewMono = Create<PlayerViewMono>(PlayerPath);
        Debug.Log(item.Value.ClientId);
        PlayerViewMono.NetworkObject.SpawnAsPlayerObject(item.Value.ClientId ,true);
      }
    }

    private void SpawnNetworkManager() => 
      NetworkManager = Create<NetworkManager>(NetworkManagerPath);
  }
}