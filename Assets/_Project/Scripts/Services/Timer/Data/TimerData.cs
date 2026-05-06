using System;

namespace _Project.Scripts.Services.Timer.Data
{
  [Serializable]
  public struct TimerData
  {
    public string Guid;
    public DateTime TargetDateTime;

    public TimerData(string guid, DateTime dateTime)
    {
      Guid = guid;
      TargetDateTime = dateTime;
    }
  }
}