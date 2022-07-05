using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Coins.Factory;
using Common;

namespace _Project.Scripts.Services.Coins
{
  public class GameCoins : IService, IGameCoins
  {
    private readonly IGameCoinsFactory _gameCoinsFactory;
    
    public int Coins { get; private set; }

    public GameCoins(IAssetProvider assetProvider)
    {
      Coins = 0; //TODO: Привязать к системе сохранений
      
      _gameCoinsFactory = new GameCoinsFactory(assetProvider);
      _gameCoinsFactory.CreateCoinsPanel();

      _gameCoinsFactory.Coins.SetCounter(Coins);
    }

    public void AddCoins(int coins)
    { 
      if(coins <= 0)
        return;
      
      Coins += coins;
      _gameCoinsFactory.Coins.SetCounter(Coins);

      SaveCoins();
    }  
    
    public void DecCoins(int coins)
    {
      if(coins <= 0)
        return;
      
      Coins -= coins;
      _gameCoinsFactory.Coins.SetCounter(Coins);

      SaveCoins();
    }

    private void SaveCoins()
    {
      //TODO: Привязать к системе сохранений
    }
  }
}