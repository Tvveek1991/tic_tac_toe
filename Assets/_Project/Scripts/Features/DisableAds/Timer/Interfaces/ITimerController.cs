using System;
using _Features.Disable_Ads_Window.Timer.Time_Providers;
using _Project.Scripts.Features.DisableAdsScreen.Timer.Interfaces;
using AppacheRemote.Scripts.Interfaces;

namespace _Features.Disable_Ads_Window.Timer.Interfaces
{
  public interface ITimerController : IService
  {
    void LaunchService();
    void CreateTimer(string guid, DateTime targetDatetime);
    void CompleteTimer(string guid);
    bool IsUnderTimer(string guid);
    Mono.Timer GetTimer(string guid);
    void AddCallback(ICallbackExecutor callbackExecutor);
    void RemoveCallback(ICallbackExecutor callbackExecutor);
    ITimeProvider TimeProvider { get; }
  }
}