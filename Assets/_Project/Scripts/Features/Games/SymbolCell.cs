using _Project.Scripts.Features.GameScreen.Serialize;
using UnityEngine;

namespace _Project.Scripts.Features.Games
{
  public class SymbolCell : MonoBehaviour
  {
    private SymbolSign _symbolSign;

    public void SetSymbol(SymbolSign symbol) =>
      _symbolSign = symbol;
    
    public SymbolSign GetSymbol =>
      _symbolSign;
  }
}