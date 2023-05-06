using System;

namespace _Project.Scripts.Services.InApps.Scripts
{
  public interface IInAppAccessor
  {
    void SetCallback(Action onInitialized);
    void Initialize();
  }
}