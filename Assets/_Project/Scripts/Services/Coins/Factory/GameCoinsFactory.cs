using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Services.Coins.Factory
{
  public class GameCoinsFactory : IGameCoinsFactory
  {
    private const string CoinsPanelPath = "UI/Common/Coins";
    private const string SceneParentPath = "Canvas";
    
    public Mono.Coins Coins { get; private set; }
    
    private readonly IAssetProvider _assetProvider;

    public GameCoinsFactory(IAssetProvider assetProvider) => 
      _assetProvider = assetProvider;

    public Mono.Coins CreateCoinsPanel()
    {
      var prefab = _assetProvider.GetResource<Mono.Coins>(CoinsPanelPath);
      Coins = Object.Instantiate(prefab, GameObject.Find(SceneParentPath).transform);
      return Coins;
    }

    public void Clear()
    {
      if(Coins)
        Object.Destroy(Coins.gameObject);
    }
  }
}