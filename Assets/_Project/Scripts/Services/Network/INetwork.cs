using Unity.Netcode;

namespace _Project.Scripts.Services.Network
{
  public interface INetwork
  {
    NetworkManager NetworkManager { get; }
    void LaunchService();
    void OnPlayerSpawn(bool isHost);
  }
}