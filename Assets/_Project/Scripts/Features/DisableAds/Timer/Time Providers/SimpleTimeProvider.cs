using System;

namespace _Features.Disable_Ads_Window.Timer.Time_Providers
{
  public class SimpleTimeProvider : ITimeProvider
  {
    public DateTime GetCurrentTime() => 
      DateTime.UtcNow;
  }
}