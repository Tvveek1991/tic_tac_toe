using _Project.Scripts.Services.InApps.Scripts;
using UnityEngine.Purchasing;

namespace _Project.Scripts.Services._Interfaces
{
  public interface IInAppCallback
  {
    public void OnPurchaseComplete(Product purchased);
    public void OnPurchaseFailed();
    public void OnNotPurchased();
    ProductId ProductsId { set; get; }
  }
  
  
}