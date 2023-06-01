namespace _Project.Scripts.Features.Loading
{
  public interface ILoadingFactory
  {
    LoadingMono LoadingMono { get; }
    void CreateScreen();
    void ClearScreen();
  }
}