using System.Collections.Generic;
using _Project.Scripts.Core_Logic.Common;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Services._Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace _Project.Scripts.Services.Network
{
  public class Network : GameObjectFactory, INetwork
  {
    private const string NetworkManagerPath = "Features/Network/Network Manager";

    public NetworkManager NetworkManager { get; private set; }

    public Network(IAssetProvider assetProvider) : base(assetProvider) { }

    private readonly List<PlayerViewMono> _playersViewMono = new List<PlayerViewMono>();
    
    public void LaunchService() => 
      SpawnNetworkManager();

    public void ConnectClient(bool isHost)
    {
      if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer) 
        return;
      
      if (isHost)
        NetworkManager.StartHost();
      else
        NetworkManager.StartClient();
    }

    public void ConnectPlayersToNetwork()
    {
      for (var i = 0; i < NetworkManager.ConnectedClients.Count; i++) 
        _playersViewMono[i].NetworkObject.SpawnAsPlayerObject(NetworkManager.ConnectedClients[(ulong) i].ClientId, true);
    }

    public void AddPlayer(PlayerViewMono playerViewMono) => 
      _playersViewMono.Add(playerViewMono); 

    private void SpawnNetworkManager() => 
      NetworkManager = Create<NetworkManager>(NetworkManagerPath);
  }
}