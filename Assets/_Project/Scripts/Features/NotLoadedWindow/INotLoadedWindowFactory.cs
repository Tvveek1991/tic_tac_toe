namespace _Project.Scripts.Features.NotLoadedWindow
{
  public interface INotLoadedWindowFactory
  {
    NotLoadedWindow SpawnNotLoadedWindow();
    void Clear();
  }
}