using System;

namespace _Features.Disable_Ads_Window.Timer.Time_Providers
{
  public interface ITimeProvider
  {
    DateTime GetCurrentTime();
  }
}