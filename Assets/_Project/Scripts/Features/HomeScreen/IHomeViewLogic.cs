using System;

namespace _Project.Scripts.Features.HomeScreen
{
  public interface IHomeViewLogic
  {
    void Construct(Action openSettingsWindow, Action toShop, Action<bool> toGame, Action toGameMode, Action toNoAds);
    string GetInputIp();
  }
}