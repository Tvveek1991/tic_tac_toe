using System;
using System.Collections.Generic;

namespace _Features.Disable_Ads_Window.Timer.Data
{
  [Serializable]
  public struct Timers
  {
    public List<TimerData> ActiveTimers;

    public Timers(List<TimerData> list) => 
      ActiveTimers = list;
  }
}