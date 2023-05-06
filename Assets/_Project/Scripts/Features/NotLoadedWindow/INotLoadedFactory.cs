namespace _Project.Scripts.Features.NotLoadedWindow
{
  public interface INotLoadedFactory
  {
    void CreateScreen();
    void ClearScreen();
    NotLoadedMono NotLoadedMono { get; }
  }
}