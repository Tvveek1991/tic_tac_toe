using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Project.Scripts.Services._data
{
  [CreateAssetMenu(fileName = "Data Unlock Base", menuName = "Data/Data Unlock Base", order = 0)]
  public class DataUnlockBase : ScriptableObject
  {
    [SerializeField] private List<UnlockData> _unlocksData;
    
    public UnlockData GetUnlockData(string guid) =>
      _unlocksData.FirstOrDefault(item => item._baseContentsData.FirstOrDefault(i => i.Id == guid));
  }
}