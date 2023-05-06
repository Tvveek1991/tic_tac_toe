using System;
using _Modules._InApps.Scripts;
using _Project.Scripts.Services.InApps.Scripts;
using UnityEngine.Purchasing;

namespace _Modules._InApps.Interfaces
{
  public interface IInAppCallback
  {
    public void OnPurchaseComplete(Product purchased);
    public void OnPurchaseFailed();
    public void OnNotPurchased();
    ProductId ProductsId { set; get; }
  }
  
  
}