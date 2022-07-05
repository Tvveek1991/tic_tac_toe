namespace _Project.Scripts.Services.Coins
{
  public interface IGameCoins
  {
    int Coins { get; }
    void AddCoins(int coins);
    void DecCoins(int coins);
  }
}