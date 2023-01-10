using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.Games.Classic.Mono
{
  public class GameFieldView : MonoBehaviour
  {
    [SerializeField] private Button _changeSignButton;
    [SerializeField] private List<SymbolCell> PlayablePoints;

    public Button ChangeSignButton => _changeSignButton;
    
    public List<SymbolCell> GetPlayablePoint => 
      PlayablePoints;
  }
}