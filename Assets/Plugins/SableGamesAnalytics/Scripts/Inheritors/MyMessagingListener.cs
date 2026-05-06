using FlurrySDK;
using UnityEngine;

namespace SableGamesAnalytics.Scripts.Inheritors
{
  public class MyMessagingListener : Flurry.IFlurryMessagingListener
  {
    public bool OnNotificationReceived(Flurry.FlurryMessage message)
    {
      Debug.Log("Flurry Messaging Notification Received: " + message.Title);
      return false;
    }

    public bool OnNotificationClicked(Flurry.FlurryMessage message)
    {
      Debug.Log("Flurry Messaging Notification Clicked: " + message.Title);
      return false;
    }

    public void OnNotificationCancelled(Flurry.FlurryMessage message) => 
      Debug.Log("Flurry Messaging Notification Cancelled: " + message.Title);

    public void OnTokenRefresh(string token) => 
      Debug.Log("Flurry Messaging Token Refresh: " + token);

    public void OnNonFlurryNotificationReceived(System.IDisposable nonFlurryMessage) => 
      Debug.Log("Flurry Messaging Non-Flurry Notification.");
  }
}