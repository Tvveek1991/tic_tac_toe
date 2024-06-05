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

      var shopView = Create<ShopView>(ShopPath, parent);
      return shopView;
    }

    public ScrollViewItemsView CreateItemsView(Transform parent)
    {
      var scrollViewItemsView =  Create<ScrollViewItemsView>(ItemsViewPath, parent);
      return scrollViewItemsView;
    }

    public ScrollViewItemButton CreateItemButton(Transform parent)
    {
      var scrollViewItemButton = Create<ScrollViewItemButton>(ItemButtonPath, parent);
      return scrollViewItemButton;
    }
  }
}