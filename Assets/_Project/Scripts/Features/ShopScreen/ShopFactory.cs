using _Project.Scripts.Core_Logic.Common;
using _Project.Scripts.Features.ScrollView;
using _Project.Scripts.Features.ShopScreen.Mono;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.ShopScreen
{
  public class ShopFactory : GameObjectFactory, IShopFactory
  {
    private const string ShopPath = "Features/ShopScreen/Shop Window";

    private const string ItemsViewPath = "Features/ShopScreen/Shop Scroll";  
    private const string ItemButtonPath = "Features/ShopScreen/Shop Item";

    private const string BaseCanvas = "Canvas";

    private readonly IAssetProvider _assetProvider;

    public ShopFactory(IAssetProvider assetProvider) : base(assetProvider){}

    public ShopView CreateShopView()
    {
      Transform parent = GameObject.Find(BaseCanvas).transform;

      var homeScreen = Create<ShopView>(ShopPath);
      return Object.Instantiate(homeScreen, parent);
    }

    public ScrollViewItemsView CreateItemsView(Transform parent)
    {
      var homeScreen =  Create<ScrollViewItemsView>(ItemsViewPath);
      return Object.Instantiate(homeScreen, parent);
    }

    public ScrollViewItemButton CreateItemButton(Transform parent)
    {
      var homeScreen = Create<ScrollViewItemButton>(ItemButtonPath);
      return Object.Instantiate(homeScreen, parent);
    }
  }
}