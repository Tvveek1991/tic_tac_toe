using System;
using System.Threading.Tasks;
using SableGamesRemote.Scripts.Data;

namespace SableGamesRemote.Scripts.Interfaces
{
  public interface IRemote : IService
  {
    Task Initialize();
    PlacementType GetPlacementType();
    string GetPrivacyText();
    event Action OnFetch;
    bool ShouldEnableFeatureTest();
    bool ShouldEnableFeatureTest2();
  }
}