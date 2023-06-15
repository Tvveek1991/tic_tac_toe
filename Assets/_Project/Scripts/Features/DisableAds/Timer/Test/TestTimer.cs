using _Features.Disable_Ads_Window.Timer.Mono;
using _Project.Scripts.Features.DisableAds.Timer;
using _Project.Scripts.Features.DisableAds.Timer.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Features.Disable_Ads_Window.Timer.Test
{
  public class TestTimer : MonoBehaviour
  {
    private ITimerController _timerController;

    private void Awake() => 
      _timerController = new TimerController();

    [Button]
    public void AddCallback(string guid)
    {
      GameObject gameObject = new GameObject("[] Test Callback");
      var testCallbackExecutor = gameObject.AddComponent<TestCallbackExecutor>();
      testCallbackExecutor.Construct(guid);
      
      _timerController.AddCallback(testCallbackExecutor);
    }
    
    [Button]
    public void Launch() => 
      _timerController.LaunchService();
    
    [Button]
    public void Complete(string guid) => 
      _timerController.CompleteTimer(guid);

    [Button]
    public void CreateTestTimer(string guid, int seconds) => 
      _timerController.CreateTimer(guid, _timerController.TimeProvider.GetCurrentTime().AddSeconds(seconds));
  }
}