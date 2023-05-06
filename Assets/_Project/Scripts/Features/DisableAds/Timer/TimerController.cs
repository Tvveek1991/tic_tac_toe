using System;
using System.Collections.Generic;
using System.Linq;
using _Features.Disable_Ads_Window.Timer.Data;
using _Features.Disable_Ads_Window.Timer.Interfaces;
using _Features.Disable_Ads_Window.Timer.Time_Providers;
using _Project.Scripts.Features.DisableAdsScreen.Timer.Interfaces;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Features.Disable_Ads_Window.Timer
{
  public class TimerController : ITimerController
  {
    private const string TimerKey = "TimerController.Timers";

    private Timers _data;
    
    private readonly List<ICallbackExecutor> _callbacks = new List<ICallbackExecutor>();
    private readonly List<Mono.Timer> _timerInstances = new List<Mono.Timer>();

    public ITimeProvider TimeProvider { get; private set; }
    
    public void LaunchService()
    {
      TimeProvider = new SimpleTimeProvider();
      
      LoadData();
      CreateInstances();
    }
    
    public void CreateTimer(string guid, DateTime targetDatetime)
    {
      if(_data.ActiveTimers == null)
        throw new Exception("Initialize Timer Controller At First");
      
      RelaunchTimer();

      if (_data.ActiveTimers.Any(p => p.Guid == guid))
      {
        Debug.Log($"Timer already exist {guid}");
        return;
      }

      var timerData = new TimerData()
      {
        Guid = guid, 
        TargetDateTime = targetDatetime
      };
      
      _data.ActiveTimers.Add(timerData);
      
      SaveData();

      //throw new Exception("Interrupt");
      CreateInstance(timerData, FindCallback(guid));
    }

    public void CompleteTimer(string guid)
    {
      if (_timerInstances.All(p => p.GetTimerGuid() != guid))
      {
        Debug.Log($"Timer not exist {guid}");
        return;
      }

      Mono.Timer monoTimer = _timerInstances.FirstOrDefault(p => p.GetTimerGuid() == guid);
      monoTimer.GetCallbackExecutor().ForEach(p => p?.ExecuteWhenComplete());

      RelaunchTimer();
    }

    public bool IsUnderTimer(string guid) => 
      _timerInstances.Any(p => p.GetTimerGuid() == guid);

    public Mono.Timer GetTimer(string guid) => 
      _timerInstances.FirstOrDefault(p => p.GetTimerGuid() == guid);

    public void AddCallback(ICallbackExecutor callbackExecutor)
    {
      if (_callbacks.Contains(callbackExecutor))
        return;
      
      _callbacks.Add(callbackExecutor);

      Mono.Timer timer = _timerInstances.FirstOrDefault(p => p.GetTimerGuid() == callbackExecutor.GetTimerGuid());
      if(timer != null)
        timer.AddCallbackExecutor(callbackExecutor);
      
      callbackExecutor.SetLock(true);
    }

    public void RemoveCallback(ICallbackExecutor callbackExecutor)
    {
      if (!_callbacks.Contains(callbackExecutor))
        return;
      
      Mono.Timer timer = _timerInstances.FirstOrDefault(p => p.GetTimerGuid() == callbackExecutor.GetTimerGuid());
      if(timer != null)
        timer.RemoveCallbackExecutor(callbackExecutor);
      
      _callbacks.Remove(callbackExecutor);
    }

    private void CreateInstances()
    {
      foreach (TimerData dataActiveTimer in _data.ActiveTimers) 
        CreateInstance(dataActiveTimer, FindCallback(dataActiveTimer.Guid));
      
      foreach (Mono.Timer timerInstance in _timerInstances) 
        Debug.Log($"Recreate {timerInstance.GetTimerGuid()} Target Date {timerInstance.GetTargetTime()}");
    }

    private void CreateInstance(TimerData timerData, ICallbackExecutor callbackExecutor)
    {
      var timer = Object.FindObjectOfType<Mono.Timer>();

      if (timer == null)
      {
        GameObject timerObject = new GameObject($"Timer {timerData.Guid}");
        timer = timerObject.AddComponent<Mono.Timer>();
      }

      timer.Construct(timerData, callbackExecutor, CompleteTimer);

      if (timerData.TargetDateTime.Subtract(TimeProvider.GetCurrentTime()).TotalSeconds <= 0)
        callbackExecutor?.ExecuteWhenComplete();
      else
      {
        _timerInstances.Add(timer);
        timer.Launch(TimeProvider.GetCurrentTime());
      }
    }

    private void RelaunchTimer()
    {
      _data.ActiveTimers.Clear();
      
      SaveData();

      Debug.Log("Destroyed");
    }

    [CanBeNull]
    private ICallbackExecutor FindCallback(string guid)
    {
      ICallbackExecutor callbackExecutor = _callbacks.FirstOrDefault(p => p.GetTimerGuid() == guid);
      
      if(callbackExecutor == null)
        Debug.Log($"Callback For {guid} Not Found");
      
      return callbackExecutor;
    }

    private void SaveData()
    {
      string value = JsonConvert.SerializeObject(_data);
      PlayerPrefs.SetString(TimerKey, value);
    }

    private void LoadData()
    {
      _data = PlayerPrefs.GetString(TimerKey, "") == "" ? 
        new Timers(new List<TimerData>()) :
        JsonConvert.DeserializeObject<Timers>(PlayerPrefs.GetString(TimerKey, ""));
    }
  }
}