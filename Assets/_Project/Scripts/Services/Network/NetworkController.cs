using _Project.Scripts.Services._Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace _Project.Scripts.Services.Network
{
  public class NetworkController : INetworkController
  {
    private const string NetworkManagerPath = "Features/Network/Network Manager";

    private readonly IAssetProvider _assetProvider;

    private NetworkManager _networkManager;

    public NetworkController(IAssetProvider assetProvider)
    {
      _assetProvider = assetProvider;
    }

    public void LaunchService()
    {
      SpawnNetworkManager();
    }

    public void StartHost()
    {
      if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        _networkManager.StartHost();
    }

    public void StartServer()
    {
      if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        _networkManager.StartServer();
    }

    public void StartClient()
    {
      if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        _networkManager.StartClient();
    }

    private void SpawnNetworkManager()
    {
      var original = _assetProvider.GetResource<NetworkManager>(NetworkManagerPath);
      _networkManager = Object.Instantiate(original);
    }
  }
}