using System;
using UnityEngine;

namespace _Project.Scripts.Features.ScrollView.Serialized
{
  [Serializable]
  public struct ScrollViewItemData
  {
    public string Id;
    public string NameKey;
    public string AdditionalNameKey;
    public string DescriptionKey;
    public Sprite Icon;
    public Sprite IconLock;
    public string DescriptionAdditional;
    public Sprite DescriptionIcon;
  }
}