using System.Collections;
using System.Net;
using System.Net.Sockets;
using _Project.Scripts.Core_Logic.Common;
using _Project.Scripts.Features.GameScreen;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Network.Mono;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace _Project.Scripts.Services.Network
{
  public class Network : GameObjectFactory, INetwork
  {
    private const string ClientDataTransmitterPath = "Features/Network/Client Data Transmitter";
    private const string NetworkManagerPath = "Features/Network/Network Manager";

    public NetworkManager NetworkManager { get; private set; }

    public Network(IAssetProvider assetProvider) : base(assetProvider) { }

    public void LaunchService()
    {
      SpawnNetworkManager();

      Debug.Log("My Local IP: " + GetLocalIPAddress());
    }

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

    public UnityTransport GetUnityTransport() => 
      NetworkManager.GetComponent<UnityTransport>();

    public string GetLocalIPAddress()
    {
      IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
      foreach (IPAddress ip in host.AddressList)
      {
        if (ip.AddressFamily == AddressFamily.InterNetwork)
          return ip.ToString();
      }
      return null;
    }

    private void SpawnNetworkManager() =>
      NetworkManager = Create<NetworkManager>(NetworkManagerPath);
  }
}