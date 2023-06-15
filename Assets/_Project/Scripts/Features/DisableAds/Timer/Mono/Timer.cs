using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using _Features.Disable_Ads_Window.Timer.Data;
using _Project.Scripts.Features.DisableAdsScreen.Timer.Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.DisableAds.Timer.Mono
{
  public class Timer /*: AppacheAds.Scripts.Mono.Singleton<Timer>*/
  {
    private readonly List<ICallbackExecutor> _callbackExecutor = new List<ICallbackExecutor>();
    private TimerData _timerData;

    public event Action<string> OnEnd;

    public void Construct(TimerData timerData, ICallbackExecutor callbackExecutor, Action<string> onEnd)
    {      
      _callbackExecutor.Clear();
      _callbackExecutor.Add(callbackExecutor);

      _timerData = timerData;
      OnEnd = onEnd;
    }
    
    public void AddCallbackExecutor(ICallbackExecutor callbackExecutor) => 
      _callbackExecutor.Add(callbackExecutor);
    
    public void RemoveCallbackExecutor(ICallbackExecutor callbackExecutor) => 
      _callbackExecutor.Remove(callbackExecutor);
    
    public void Launch(DateTime startDatetime)
    {
      // StopAllCoroutines();
      // StartCoroutine(Tick(startDatetime));
    }

    public string GetTimerGuid() => 
      _timerData.Guid;
    
    public string GetTargetTime() => 
      _timerData.TargetDateTime.ToString(CultureInfo.InvariantCulture);

    public List<ICallbackExecutor> GetCallbackExecutor() => 
      _callbackExecutor;

    IEnumerator Tick(DateTime startDatetime)
    {
      TimeSpan timeSpan = _timerData.TargetDateTime.Subtract(startDatetime);
      int seconds = Mathf.RoundToInt((float) timeSpan.TotalSeconds);
      
      _callbackExecutor.ForEach(p => p?.ExecuteWhenTick(seconds));
      
      while (seconds > 0)
      {
        yield return new WaitForSecondsRealtime(1);;
        seconds--;    
        _callbackExecutor.ForEach(p => p?.ExecuteWhenTick(seconds));
      }
      
      _callbackExecutor.ForEach(p => p?.ExecuteWhenTick(seconds));
      OnEnd?.Invoke(_timerData.Guid);
    }

    private void OnDestroy()
    {
      Debug.Log("Oh no Destroyed");
    }
  }
}