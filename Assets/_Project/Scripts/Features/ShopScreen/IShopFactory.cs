using _Project.Scripts.Features.ScrollView;
using _Project.Scripts.Features.ShopScreen.Mono;
using UnityEngine;

namespace _Project.Scripts.Features.ShopScreen
{
  public interface IShopFactory
  {
    ShopView CreateShopView();
    ScrollViewItemsView CreateItemsView(Transform parent);
    ScrollViewItemButton CreateItemButton(Transform parent);
  }
}