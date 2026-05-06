namespace _Project.Scripts.Features.HomeScreen
{
  public interface IHomeFactory
  {
    void CreateHomeView();
    void ClearHomeView();
    IHomeViewLogic HomeViewLogic { get; }
  }
}