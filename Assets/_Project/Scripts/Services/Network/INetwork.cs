using _Project.Scripts.Features.GameScreen;
using _Project.Scripts.Services._Interfaces;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

namespace _Project.Scripts.Services.Network
{
  public interface INetwork
  {
    NetworkManager NetworkManager { get; }
    void ConnectClient(bool isHost);
    void LaunchService();
    void CreateClientDataTransmitter(IGameFactory metaProvider, IGameMetaProvider gameMetaProvider);
    string GetLocalIPAddress();
    UnityTransport GetUnityTransport();
  }
}