using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using _Project.Scripts.Services.Timer.Data;
using _Project.Scripts.Timer.Data;
using _Project.Scripts.Timer.Interfaces;
using UnityEngine;

namespace _Project.Scripts.Services.Timer.Mono
{
  public class Timer : MonoBehaviour
  {
    private List<ICallbackExecutor> _callbackExecutor = new List<ICallbackExecutor>();
    private TimerData _timerData;
    
    public event Action<string> OnEnd;
    public event Action<int> OnTick;

    public void Construct(TimerData timerData, ICallbackExecutor callbackExecutor, Action<string> onEnd)
    {      
      _callbackExecutor.Add(callbackExecutor);

      _timerData = timerData;
      OnEnd = onEnd;
    }

    public void AddCallbackExecutor(ICallbackExecutor callbackExecutor) => 
      _callbackExecutor.Add(callbackExecutor);
    
    public void RemoveCallbackExecutor(ICallbackExecutor callbackExecutor) => 
      _callbackExecutor.Remove(callbackExecutor);
    
    public void Launch(DateTime startDatetime) => 
      StartCoroutine(Tick(startDatetime));

    public string GetTimerGuid() => 
      _timerData.Guid;
    
    public string GetTargetTime() => 
      _timerData.TargetDateTime.ToString(CultureInfo.InvariantCulture);

    public List<ICallbackExecutor> GetCallbackExecutor() => 
      _callbackExecutor;

    public TimeSpan GetSubtractValue(DateTime startDatetime) =>
      _timerData.TargetDateTime.Subtract(startDatetime);

    IEnumerator Tick(DateTime startDatetime)
    {
      var waitForSecondsRealtime = new WaitForSecondsRealtime(1);

      TimeSpan timeSpan = _timerData.TargetDateTime.Subtract(startDatetime);
      int seconds = Mathf.RoundToInt((float) timeSpan.TotalSeconds);
      
      _callbackExecutor.ForEach(p => p?.ExecuteWhenTick(seconds));
      OnTick?.Invoke(seconds);

      while (seconds > 0)
      {
        yield return waitForSecondsRealtime;
        seconds--;    
        _callbackExecutor.ForEach(p => p?.ExecuteWhenTick(seconds));
        OnTick?.Invoke(seconds);
      }
      
      _callbackExecutor.ForEach(p => p?.ExecuteWhenTick(seconds));
      OnTick?.Invoke(seconds);
      OnEnd?.Invoke(_timerData.Guid);
    }
  }
}