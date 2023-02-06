using System.Collections.Generic;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Services.Network.Mono;
using Unity.Netcode;

namespace _Project.Scripts.Services.Network
{
  public interface INetwork
  {
    NetworkManager NetworkManager { get; }
    void ConnectClient(bool isHost);
    void LaunchService();
    void ConnectPlayersToNetwork();
    void AddPlayer(PlayerViewMono playerViewMono);
  }
}