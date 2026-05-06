using _Project.Scripts.Features.GameScreen.Serialized;

namespace _Project.Scripts.Features.Data
{
  public class DragableData
  {
    private readonly SymbolSign _sign;
    private readonly SymbolSize _size;

    public bool IsBot { get; }
    
    public DragableData(SymbolSign sign, SymbolSize size, bool isBot = false)
    {
      _sign = sign;
      _size = size;
      
      IsBot = isBot;
    }

    public SymbolSign GetSymbol() =>
      _sign;

    public SymbolSize GetSize() =>
      _size;

  }
}