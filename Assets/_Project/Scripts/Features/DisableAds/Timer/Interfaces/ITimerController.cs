using System;
using _Features.Disable_Ads_Window.Timer.Time_Providers;
using _Project.Scripts.Features.DisableAdsScreen.Timer.Interfaces;
using _Project.Scripts.Service_Base;

namespace _Project.Scripts.Features.DisableAds.Timer.Interfaces
{
  public interface ITimerController : IService
  {
    void LaunchService();
    void CreateTimer(string guid, DateTime targetDatetime);
    void CompleteTimer(string guid);
    bool IsUnderTimer(string guid);
    _Project.Scripts.Features.DisableAds.Timer.Mono.Timer GetTimer(string guid);
    void AddCallback(ICallbackExecutor callbackExecutor);
    void RemoveCallback(ICallbackExecutor callbackExecutor);
    ITimeProvider TimeProvider { get; }
  }
}