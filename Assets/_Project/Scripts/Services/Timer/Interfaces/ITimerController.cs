using System;
using _Project.Scripts.Timer.Interfaces;

namespace _Project.Scripts.Services.Timer.Interfaces
{
  public interface ITimerController
  {
    void LaunchService();
    void CreateTimer(string guid, DateTime targetDatetime);
    void CompleteTimer(string guid);
    bool IsUnderTimer(string guid);
    Services.Timer.Mono.Timer GetTimer(string guid);
    void AddCallback(ICallbackExecutor callbackExecutor);
    void RemoveCallback(ICallbackExecutor callbackExecutor);
    void Clean();
    ITimeProvider TimeProvider { get; }
    TimeSpan GetSubtractValue(string guid);
  }
}