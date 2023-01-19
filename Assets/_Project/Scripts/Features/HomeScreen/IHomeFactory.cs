namespace _Project.Scripts.Features.HomeScreen
{
  public interface IHomeFactory
  {
    Mono.HomeView HomeView { get; }
    void CreateHomeView();
    void ClearHomeView();
  }
}