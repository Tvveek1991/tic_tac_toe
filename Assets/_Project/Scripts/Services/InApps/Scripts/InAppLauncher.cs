using System;
using System.Collections.Generic;
using _Project.Scripts.Services._Interfaces;
using UnityEngine.Purchasing;
using Product = _Modules._InApps.Scripts.Main.Product;

namespace _Project.Scripts.Services.InApps.Scripts
{
  public class InAppLauncher : IInAppAccessor
  {
    private readonly IInAppService _inAppService;

    public InAppLauncher(IInAppService inAppService)
    {
      _inAppService = inAppService;

      AddNoAdsProduct();
    }

    public void Initialize() => 
      _inAppService.Initialize();

    public void SetCallback(Action onInitialized) =>
      _inAppService.OnInitializedEvent += onInitialized;

    private void AddNoAdsProduct()
    {
      _inAppService.AddProduct(new Product
      {
        ProductName = ProductId.storytale_premium.ToString(),
        ProductType = ProductType.NonConsumable,
        IDs = new Dictionary<string, string>
        {
          {ProductId.storytale_premium.ToString(), GooglePlay.Name}
        }
      });
    }
  }
}