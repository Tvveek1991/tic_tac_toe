using _Project.Scripts.Features.Player;
using _Project.Scripts.Services.Network.Mono;
using Unity.Netcode;

namespace _Project.Scripts.Services.Network
{
  public interface INetwork
  {
    NetworkManager NetworkManager { get; }
    PlayerViewMono PlayerViewMono { get; }
    void OnPlayerSpawn(bool isHost);
    void LaunchService();
    void SpawnPlayers();
  }
}