using System;
using System.Collections.Generic;
using _Project.Scripts.Services.Timer.Data;

namespace _Project.Scripts.Timer.Data
{
  [Serializable]
  public struct Timers
  {
    public List<TimerData> ActiveTimers;

    public Timers(List<TimerData> list) => 
      ActiveTimers = list;
  }
}