using System;
using _Project.Scripts.Features.Customization.Serialized;

namespace _Project.Scripts.Features.Customization.Interface
{
  public interface ISkins
  {
    Action<ISkinData> OnSkinActivated { get; set; }
    Action<ISkinData> OnSkinReceived { get; set; }

    ISkinData[] GetSkinDatasByType(SkinType type);

    void Show();
    void Clean();
    void AchieveAllSkins();
  }
}