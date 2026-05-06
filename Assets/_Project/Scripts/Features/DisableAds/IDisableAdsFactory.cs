using System;
using _Project.Scripts.Features.DisableAds.Mono;

namespace _Project.Scripts.Features.DisableAds
{
  public interface IDisableAdsFactory
  {
    IDisableAdsViewLogic SpawnDisableAdsWindow();
    void Clear();
  }
}