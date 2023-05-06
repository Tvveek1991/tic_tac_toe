using _Features.Disable_Ads_Window.Timer.Interfaces;
using _Project.Scripts.Features.DisableAdsScreen.Timer.Interfaces;
using UnityEngine;

namespace _Features.Disable_Ads_Window.Timer.Mono
{
  public class TestCallbackExecutor : MonoBehaviour, ICallbackExecutor
  {
    private int _seconds;
    private string _guid;

    public void Construct(string guid) => 
      _guid = guid;

    private void OnGUI()
    {
      GUI.Label(new Rect(10, 10, 100, 20), $"Timer {_seconds}");
    }

    public string GetTimerGuid() => 
      _guid;

    public void ExecuteWhenTick(int seconds) => 
      _seconds = seconds;

    public void ExecuteWhenComplete()
    {
      Debug.Log("Complete!!");
      Destroy(gameObject);
    }

    public void SetLock(bool isLocked)
    {}

    public void Destroy()
    {}

    public void Initialize()
    {
      throw new System.NotImplementedException();
    }
  }
}