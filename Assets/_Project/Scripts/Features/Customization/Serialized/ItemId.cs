using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts.MonoBehaviours.Game
{
  public class ItemId : MonoBehaviour
  {
    public string Id
    {
      get => _guid;
      set => _guid = value;
    }
    
    [SerializeField] private string _guid = string.Empty;
  }
}