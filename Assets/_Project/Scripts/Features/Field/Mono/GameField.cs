using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Features.Field.Mono
{
  public class GameField : MonoBehaviour
  {
    [SerializeField] private List<SymbolCell> PlayablePoints;
    
    public List<SymbolCell> GetPlayablePoint => 
      PlayablePoints;
  }
}