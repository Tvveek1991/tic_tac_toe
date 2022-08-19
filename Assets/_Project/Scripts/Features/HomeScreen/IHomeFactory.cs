namespace _Project.Scripts.Features.HomeScreen
{
  public interface IHomeFactory
  {
    Mono.HomeView HomeView { get; }
    Mono.HomeView CreateHomeView();
    void ClearHomeView();
  }
}