using System;
using _Project.Scripts.Service_Base;
using Product = _Modules._InApps.Scripts.Main.Product;

namespace _Project.Scripts.Services._Interfaces
{
  public interface IInAppService : IService
  {
    void Initialize();
    void Purchase(string productId);
    void AddProduct(Product productInfo);
    void AddInAppCallback(IInAppCallback inAppCallback);
    void RemoveInAppCallback(IInAppCallback inAppCallback);

    string GetPrice(string productId);
    float GetPriceFloat(string productId);
    bool IsProductInitialized(string productId);

    event Action OnInitializedEvent;
    event Action OnInitializeFailEvent;
    bool IsInAppsInitialized();
    bool IsProductPurchased(string id);
    bool IsSubscriptionPurchased(); //классифицировать, если будет много разных по логике подписок
  }
}