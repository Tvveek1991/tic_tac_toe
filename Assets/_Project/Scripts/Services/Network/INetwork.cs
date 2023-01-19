namespace _Project.Scripts.Services.Network
{
  public interface INetwork
  {
    void LaunchService();
    void OnPlayerSpawn(bool isHost);
  }
}