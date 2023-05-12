using System;
using System.Collections.Generic;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Services.Unlocker;
using UnityEngine;

namespace _Project.Scripts.Services._data
{
  [CreateAssetMenu(fileName = "Unlock Data", menuName = "Data/Unlock Data", order = 0)]
  public class UnlockData : ScriptableObject
  {
    public UnlockType Type;
    public uint Cost;

    public List<BaseContentData> _baseContentsData;
  }
}