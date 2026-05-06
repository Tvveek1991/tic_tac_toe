using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts.Features.Data
{
  [CreateAssetMenu(fileName = "Base Content Data", menuName = "Data/Base Content Data", order = 0)]
  public abstract class BaseContentData : ScriptableObject
  {
    public string Id;
    public string Name;
    public string Description;
    
#if UNITY_EDITOR
    [Button]
    public void GenerateGuid() =>
      Id = Guid.NewGuid().ToString();
#endif

  }
}