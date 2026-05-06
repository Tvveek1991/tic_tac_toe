using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Services.Timer.Data;
using _Project.Scripts.Services.Timer.Interfaces;
using _Project.Scripts.Timer.Data;
using _Project.Scripts.Timer.Interfaces;
using _Project.Scripts.Timer.Time_Providers;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Services.Timer
{
  public class TimerController : ITimerController
  {
    private const string TimerKey = "TimerController.Timers";

    private Timers _data;

    private readonly List<ICallbackExecutor> _callbacks = new List<ICallbackExecutor>();
    private readonly List<Services.Timer.Mono.Timer> _timerInstances = new List<Services.Timer.Mono.Timer>();
    private readonly List<string> _guidsForRemove = new List<string>();

    public ITimeProvider TimeProvider { get; private set; }

    public void LaunchService()
    {
      TimeProvider = new SimpleTimeProvider();

      LoadData();
      CreateInstances();
    }

    public void CreateTimer(string guid, DateTime targetDatetime)
    {
      if (_data.ActiveTimers == null)
        throw new Exception("Initialize Timer Controller At First");

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

      CreateInstance(timerData, FindCallback(guid));
    }

    public TimeSpan GetSubtractValue(string guid) =>
      GetTimer(guid).GetSubtractValue(TimeProvider.GetCurrentTime());
    
    public void CompleteTimer(string guid)
    {
      if (_timerInstances.All(p => p.GetTimerGuid() != guid))
      {
        Debug.Log($"Timer not exist {guid}");
        return;
      }

      Services.Timer.Mono.Timer monoTimer = _timerInstances.FirstOrDefault(p => p.GetTimerGuid() == guid);
      monoTimer.GetCallbackExecutor().ForEach(p => p?.ExecuteWhenComplete());

      RemoveTimer(guid);
    }

    public bool IsUnderTimer(string guid) =>
      _timerInstances.Any(p => p.GetTimerGuid() == guid);

    public Services.Timer.Mono.Timer GetTimer(string guid) =>
      _timerInstances.FirstOrDefault(p => p.GetTimerGuid() == guid);

    public void AddCallback(ICallbackExecutor callbackExecutor)
    {
      if (_callbacks.Contains(callbackExecutor))
        return;

      _callbacks.Add(callbackExecutor);

      Services.Timer.Mono.Timer timer = _timerInstances.FirstOrDefault(p => p.GetTimerGuid() == callbackExecutor.GetTimerGuid());
      if (timer != null)
        timer.AddCallbackExecutor(callbackExecutor);

      callbackExecutor.SetLock(true);
    }

    public void RemoveCallback(ICallbackExecutor callbackExecutor)
    {
      if (!_callbacks.Contains(callbackExecutor))
        return;

      Services.Timer.Mono.Timer timer = _timerInstances.FirstOrDefault(p => p.GetTimerGuid() == callbackExecutor.GetTimerGuid());
      if (timer != null)
        timer.RemoveCallbackExecutor(callbackExecutor);

      _callbacks.Remove(callbackExecutor);
    }

    public void Clean()
    {
      foreach (ICallbackExecutor callbackExecutor in _callbacks)
        callbackExecutor.Destroy();

      foreach (Services.Timer.Mono.Timer timerInstance in _timerInstances)
        Object.Destroy(timerInstance);

      _timerInstances.Clear();
      _callbacks.Clear();
    }

    private void CreateInstances()
    {
      foreach (TimerData dataActiveTimer in _data.ActiveTimers)
        CreateInstance(dataActiveTimer, FindCallback(dataActiveTimer.Guid));

      CleanUpActiveTimers();

      foreach (Services.Timer.Mono.Timer timerInstance in _timerInstances)
        Debug.Log($"Recreate {timerInstance.GetTimerGuid()} Target Date {timerInstance.GetTargetTime()}");
    }

    private void CreateInstance(TimerData timerData, ICallbackExecutor callbackExecutor)
    {
      GameObject timer = new GameObject($"Timer {timerData.Guid}");

      var timerComponent = timer.AddComponent<Services.Timer.Mono.Timer>();
      timerComponent.Construct(timerData, callbackExecutor, CompleteTimer);

      if (timerData.TargetDateTime.Subtract(TimeProvider.GetCurrentTime()).TotalSeconds <= 0)
      {
        callbackExecutor?.ExecuteWhenComplete();
        _guidsForRemove.Add(timerData.Guid);
      }
      else
      {
        _timerInstances.Add(timerComponent);
        timerComponent.Launch(TimeProvider.GetCurrentTime());
      }
    }

    private void RemoveTimer(string guid)
    {
      Services.Timer.Mono.Timer timer = _timerInstances.FirstOrDefault(p => p.GetTimerGuid() == guid);

      if (timer != null)
      {
        if (_timerInstances.All(p => p.GetTimerGuid() != guid))
        {
          Debug.Log($"Cant remove {guid}");
          return;
        }
      }

      if (timer != null)
        _timerInstances.Remove(timer);

      _data.ActiveTimers.Remove(_data.ActiveTimers.FirstOrDefault(p => p.Guid == guid));

      SaveData();

      if (timer != null)
        Object.Destroy(timer.gameObject);
    }

    [CanBeNull]
    private ICallbackExecutor FindCallback(string guid)
    {
      ICallbackExecutor callbackExecutor = _callbacks.FirstOrDefault(p => p.GetTimerGuid() == guid);

      if (callbackExecutor == null)
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
      _data = PlayerPrefs.GetString(TimerKey, "") == "" ? new Timers(new List<TimerData>()) : JsonConvert.DeserializeObject<Timers>(PlayerPrefs.GetString(TimerKey, ""));
    }

    private void CleanUpActiveTimers()
    {
      foreach (string guid in _guidsForRemove)
        _data.ActiveTimers.Remove(_data.ActiveTimers.FirstOrDefault(p => p.Guid == guid));

      _guidsForRemove.Clear();
    }
  }
}