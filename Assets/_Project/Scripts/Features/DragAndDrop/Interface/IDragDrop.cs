using _Project.Scripts.Features.Player.BuhurtModules;

namespace _Project.Scripts.Features.DragAndDrop.Interface
{
  public interface IDragDrop
  {
    void BindPartToCursor(DragablePart dragablePart);
    DragablePart CurrentActiveDragPart { get; }
  }
}