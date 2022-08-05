namespace _Project.Scripts.Features.HomeScreen
{
  public interface IHomeFactory
  {
    Mono.HomeScreen HomeScreen { get; }
    Mono.HomeScreen CreateHomeScreen();
    void ClearHomeScreen();
  }
}