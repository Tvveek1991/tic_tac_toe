using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;
using UnityEngine.Purchasing;
using Product = _Modules._InApps.Scripts.Main.Product;

namespace _Project.Scripts.Services.InApps.Scripts.Main
{
  public class InAppService : IStoreListener, IInAppService
  {
    private IStoreController _controller;
    private readonly ConfigurationBuilder _builder;

    private readonly List<IInAppCallback> _inAppCallbacks = new List<IInAppCallback>();

    public event Action OnInitializedEvent;

    public event Action OnInitializeFailEvent;

    private bool _isInitialized;

    public InAppService() =>
      _builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

    public void Initialize()
    {
      AddProduct(new Product
      {
        ProductName = ProductId.noads.ToString(),
        ProductType = ProductType.NonConsumable,
        IDs = new Dictionary<string, string>
        {
          {ProductId.noads.ToString(), GooglePlay.Name}
        }
      });
      
      UnityPurchasing.Initialize(this, _builder);
    }

    public void Purchase(string productId) =>
      _controller.InitiatePurchase(productId);

    public void AddProduct(Product productInfo)
    {
      var storeIDs = new IDs();

      foreach (KeyValuePair<string, string> id in productInfo.IDs)
        storeIDs.Add(id.Key, id.Value);

      _builder.AddProduct(productInfo.ProductName, productInfo.ProductType, storeIDs);
    }

    public bool IsProductInitialized(string productId)
    {
      Debug.Log("[InApp] IsProductInitialized Called");
      return _controller?.products?.WithID(productId) != null;
    }

    public bool IsInAppsInitialized()
    {
      Debug.Log("[InApp] IsInAppsInitialized Called");
      return _isInitialized;
    }

    public string GetPrice(string productId)
    {
      if (_controller == null)
        return "0";
      
      return _controller.products.WithID(productId).metadata.localizedPriceString;
    }
    
    public float GetPriceFloat(string productId)
    {
      Debug.Log($"[InApp] Get Price Decimal For {_controller.products.WithID(productId).metadata.localizedTitle}");
      return (float)_controller.products.WithID(productId).metadata.localizedPrice;
    }

    public void AddInAppCallback(IInAppCallback inAppCallback) =>
      _inAppCallbacks.Add(inAppCallback);
    
    public void RemoveInAppCallback(IInAppCallback inAppCallback)
    {
      if(_inAppCallbacks.Contains(inAppCallback))
        _inAppCallbacks.Remove(inAppCallback);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
      _isInitialized = true;

      _controller = controller;

      FindNotPurchasedProducts();
      
      OnInitializedEvent?.Invoke();

      Debug.Log("[InApp] Initialized");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
      OnInitializeFailEvent?.Invoke();

      Debug.Log("[InApp] Initialize Fail");

      foreach (IInAppCallback inAppCallback in _inAppCallbacks)
        inAppCallback.OnNotPurchased();
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message) { }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
      ExecutePurchasedProducts(e);
      return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(UnityEngine.Purchasing.Product product, PurchaseFailureReason failureReason) =>
      FailPurchaseProducts(product);
    
    public bool IsProductPurchased(string id)
    {
      if (_controller?.products != null)
        return _controller.products.WithID(id) != null && _controller.products.WithID(id).hasReceipt;

      return false;
    }

    public bool IsSubscriptionPurchased()
    {
      if (_isInitialized)
      {
        return _controller.products.all
          .Where(p => p.definition.type == ProductType.Subscription)
          .Any(p => p.hasReceipt);
      }

      return false;
    }

    private void ExecutePurchasedProducts(PurchaseEventArgs e)
    {
      foreach (IInAppCallback inAppCallback in _inAppCallbacks)
      {
        if (e.purchasedProduct.definition.id == inAppCallback.ProductsId.ToString())
          inAppCallback.OnPurchaseComplete(e.purchasedProduct);
      }
    }

    private void FindNotPurchasedProducts()
    {
      foreach (IInAppCallback inAppCallback in _inAppCallbacks)
      {
        if (!IsProductPurchased(inAppCallback.ProductsId.ToString()))
          inAppCallback.OnNotPurchased();
      }
    }

    private void FailPurchaseProducts(UnityEngine.Purchasing.Product product)
    {
      foreach (IInAppCallback inAppCallback in _inAppCallbacks)
      {
        if (product.definition.id == inAppCallback.ProductsId.ToString())
          inAppCallback.OnPurchaseFailed();
      }
    }
  }
}