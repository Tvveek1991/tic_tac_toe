using _Project.Scripts.Core_Logic.Common;
using _Project.Scripts.Features.GameScreen;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Network.Mono;
using Unity.Netcode;

namespace _Project.Scripts.Services.Network
{
  public class Network : GameObjectFactory, INetwork
  {
    private const string ClientDataTransmitterPath = "Features/Network/Client Data Transmitter";
    private const string NetworkManagerPath = "Features/Network/Network Manager";

    public NetworkManager NetworkManager { get; private set; }

    public Network(IAssetProvider assetProvider) : base(assetProvider) { }

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

    public void CreateClientDataTransmitter(IGameFactory gameFactory, IGameMetaProvider gameMetaProvider)
    {
      if (ClientDataTransmitter.Instance != null) return;
      
      var clientDataTransmitter = Create<ClientDataTransmitter>(ClientDataTransmitterPath);
      clientDataTransmitter.Construct(gameFactory, gameMetaProvider);
    }

    private void SpawnNetworkManager() =>
      NetworkManager = Create<NetworkManager>(NetworkManagerPath);
  }
}