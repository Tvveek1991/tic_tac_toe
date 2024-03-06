using System;

namespace _Project.Scripts.Services.Consent
{
  public interface ICmpManager
  {
    void Request(Action onEnd);
    void ResetConsent();
    bool IsFormAvailable();
  }
}