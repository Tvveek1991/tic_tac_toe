using System.Collections.Generic;
using UnityEngine.Purchasing;

namespace _Modules._InApps.Scripts.Main
{
  public struct Product
  {
    public string ProductName;
    public ProductType ProductType;
    public Dictionary<string, string> IDs;
  }

  [System.Serializable]
  public struct Receipt
  {
    public string Store;
    public string TransactionID;
    public string Payload;
  }

// Additional information about the IAP for Android.
  [System.Serializable]
  public struct PayloadAndroid
  {
    public string Json;
    public string Signature;
  }
}