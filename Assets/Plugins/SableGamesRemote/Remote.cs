using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using SableGamesRemote.Scripts.Data;
using SableGamesRemote.Scripts.Interfaces;
using SableGamesRemote.Scripts.Logger;
using SableGamesRemote.Scripts.Tools;
using UnityEngine;
using RemoteSettings = SableGamesRemote.Scripts.Data.RemoteSettings;

namespace SableGamesRemote
{
  public class Remote : IRemote
  {
    private string _privacy;
    private string _placementVariable;

    public event Action OnFetch;
    
    private RemoteSettings _remoteSettings;
    
    private readonly IRemoteLogger _remoteLogger;
    private readonly IRemoteSwitcher _remoteSwitcher;
    
    public Remote()
    {
      _remoteLogger = new RemoteLogger();
      _remoteSwitcher = new GameObject("RemoteSwitcher").AddComponent<RemoteSwitcher>();
      
      LoadSettings();
      
      InitializeAbVariable();
      SetupDefaults();
    }

    public async Task Initialize() => 
      await FetchDataAsync();

    public PlacementType GetPlacementType()
    {
      PlacementType placementType;
 
      string value = FirebaseRemoteConfig.DefaultInstance.GetValue(_placementVariable).StringValue;

      try
      {
        placementType = (PlacementType) Enum.Parse(typeof(PlacementType), value, true);
      }
      catch (ArgumentException e)
      {
        placementType = PlacementType.@default;
      }

      if (_remoteSwitcher.TestModeCounter == 0)
        return PlacementType.test;
          
      return placementType;
    }

    public string GetPrivacyText() =>
      FirebaseRemoteConfig.DefaultInstance.GetValue(_privacy).StringValue;

    public bool ShouldEnableFeatureTest()
    {    
      if (_remoteSwitcher.TestModeCounter == 0)
        return true;
      
      return GetPlacementType() == PlacementType.test || GetPlacementType() == PlacementType.test_win;
    }

    public bool ShouldEnableFeatureTest2()
    {
      if (_remoteSwitcher.TestModeCounter == 0)
        return true;
      
      return GetPlacementType() == PlacementType.test2 || GetPlacementType() == PlacementType.test2_win;
    }

    private void LoadSettings()
    {
      _remoteSettings = Resources.Load<RemoteSettings>("Sable Games Remote Settings");

      if (_remoteSettings == null) 
        _remoteLogger.ThrowNotSetupException();
      else
        _remoteLogger.SetSettings(_remoteSettings);
    }

    private void InitializeAbVariable()
    {
      _placementVariable = $"AB_{_remoteSettings.ProjectNameForVariable}_v{Application.version.Replace(".", "_")}";
      
      _remoteLogger.LogWithoutFormat($"InitializeAbVariable {_placementVariable}");

      _privacy = _remoteSettings.PrivacyPolicyVariable;
    }

    private void SetupDefaults()
    {
      Dictionary<string, object> defaults =
        new Dictionary<string, object>
        {
          {_placementVariable, PlacementType.@default.ToString()},
          {_privacy, "base"},
        };

      FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
        .ContinueWithOnMainThread(task => { });
    }

    private async Task FetchDataAsync()
    {
      _remoteLogger.LogWithoutFormat("Fetching data...");
      
      Task fetchTask =
        FirebaseRemoteConfig.DefaultInstance.FetchAsync(
          TimeSpan.Zero);

      await fetchTask.ContinueWithOnMainThread(obj =>
      {
        FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(task =>
        {
          string stringValue = FirebaseRemoteConfig.DefaultInstance.GetValue(_placementVariable).StringValue;
          string privacyValue = FirebaseRemoteConfig.DefaultInstance.GetValue(_privacy).StringValue;
          
          _remoteLogger.LogWithoutFormat($"Remote Fetched! {stringValue}");
          _remoteLogger.LogWithoutFormat($"Remote Fetched! {privacyValue}");
          
          OnFetch?.Invoke();
        });
      });
    }
  }
}