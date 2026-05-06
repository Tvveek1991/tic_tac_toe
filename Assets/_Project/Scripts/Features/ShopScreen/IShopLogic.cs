using System;

namespace _Project.Scripts.Features.ShopScreen
{
  public interface IShopLogic
  {
    void CreateShopView(Action onCloseOverride = null);
    void ClearShopView();
  }
}