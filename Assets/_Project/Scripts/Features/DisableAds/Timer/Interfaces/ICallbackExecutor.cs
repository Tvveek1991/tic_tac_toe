namespace _Project.Scripts.Features.DisableAdsScreen.Timer.Interfaces
{
  public interface ICallbackExecutor
  {
    string GetTimerGuid();
    void ExecuteWhenTick(int seconds);
    void ExecuteWhenComplete();
    void SetLock(bool isLocked);
    void Destroy();
  }
}