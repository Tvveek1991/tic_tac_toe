using _Project.Scripts.Features.Customization.Mono;
using _Project.Scripts.Features.Customization.Serialized;
using UnityEngine;

namespace _Project.Scripts.Features.Customization
{
  public interface ISkinsFactory
  {
    SkinsConfig GetSkinsConfig();
    SkinsView CreateSkinsView();
    ApproveWindow CreateApproveWindow(Transform parent);
    GameObject CreateScrollView(Transform parent);
    SkinPanel CreateSkinPanel(Transform parent);
    void Clear();
  }
}