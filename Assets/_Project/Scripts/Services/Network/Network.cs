using _Project.Scripts.Core_Logic.Common;
using _Project.Scripts.Services._Interfaces;
using Unity.Netcode;

namespace _Project.Scripts.Services.Network
{
  public class Network : GameObjectFactory, INetwork
  {
    private const string NetworkManagerPath = "Features/Network/Network Manager";

    private NetworkManager _networkManager;

    public Network(IAssetProvider assetProvider) : base(assetProvider) { }

    public void LaunchService() => 
      SpawnNetworkManager();

    public void OnPlayerSpawn(bool isHost)
    {
      if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer) 
        return;
      
      if (isHost)
        _networkManager.StartHost();
      else
        _networkManager.StartClient();
    }

    /*public void StartServer()
    {
      if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        _networkManager.StartServer();
    }*/

    private void SpawnNetworkManager() => 
      _networkManager = Create<NetworkManager>(NetworkManagerPath);
  }
}