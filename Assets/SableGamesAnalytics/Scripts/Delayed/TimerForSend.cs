using System;
using System.Globalization;
using UnityEngine;

namespace SableGamesAnalytics.Scripts.Delayed
{
  public static class TimerForSend
  {
    private const string RetentionTargetDateTime = "Retention.TargetDateTime";

    private static DateTime TargetDate
    {
      get => DateTime.Parse(PlayerPrefs.GetString(RetentionTargetDateTime, DateTime.MinValue.ToString(CultureInfo.InvariantCulture)), CultureInfo.InvariantCulture);
      set => PlayerPrefs.SetString(RetentionTargetDateTime, value.ToString(CultureInfo.InvariantCulture));
    }
    
    public static void SetTime(DateTime targetTime)
    {
      if(TargetDate != DateTime.MinValue)
        return;

      TargetDate = targetTime;
    }

    public static void CheckoutTime(Action sendIfExpired)
    {
      if(TargetDate == DateTime.MinValue)
        return;

      if (TargetDate.Subtract(DateTime.UtcNow).TotalSeconds > 0) 
        return;
      
      if(TargetDate.AddHours(24).Subtract(DateTime.UtcNow).TotalSeconds < 0)
        return;

      sendIfExpired?.Invoke();
      TargetDate = DateTime.MinValue;
    }
  }
}