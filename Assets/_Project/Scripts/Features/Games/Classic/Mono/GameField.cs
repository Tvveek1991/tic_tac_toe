using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Features.Games.Classic.Mono
{
  public class GameField : MonoBehaviour
  {
    [SerializeField] private List<SymbolCell> PlayablePoints;
    
    public List<SymbolCell> GetPlayablePoint => 
      PlayablePoints;
  }
}