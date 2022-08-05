using System;
using _Project.Scripts.Services.Timer.Interfaces;
using _Project.Scripts.Timer.Interfaces;

namespace _Project.Scripts.Timer.Time_Providers
{
  public class SimpleTimeProvider : ITimeProvider
  {
    public DateTime GetCurrentTime() => 
      DateTime.UtcNow;
  }
}