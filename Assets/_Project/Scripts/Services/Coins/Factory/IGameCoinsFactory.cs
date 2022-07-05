namespace _Project.Scripts.Services.Coins.Factory
{
  public interface IGameCoinsFactory
  {
    Mono.Coins Coins { get; }
    Mono.Coins CreateCoinsPanel();
    void Clear();
  }
}