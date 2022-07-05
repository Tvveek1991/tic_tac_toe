using System;
using UnityEngine;

namespace _Project.Scripts.Features.Data
{
  [CreateAssetMenu(fileName = "Base Content Data", menuName = "Data/Base Content Data", order = 0)]
  public abstract class BaseContentData : ScriptableObject
  {
    public Guid Id;
    public string Name;
    public string Description;
  }
}