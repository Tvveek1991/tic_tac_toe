using System;
using System.Threading.Tasks;
using SableGamesRemote.Scripts.Data;
using SableGamesRemote.Scripts.Interfaces;

namespace SableGamesRemote
{
  public class RemoteDummy : IRemote
  {
    public event Action OnFetch;
    public bool ShouldEnableFeatureTest() => 
      true;

    public bool ShouldEnableFeatureTest2() => 
      true;

    public Task Initialize()
    {
      OnFetch?.Invoke();
      return Task.CompletedTask;
    }

    public PlacementType GetPlacementType() => 
      PlacementType.@default;

    public string GetPrivacyText() => 
      "";
  }
}