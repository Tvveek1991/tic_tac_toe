using System;
using System.Threading.Tasks;
using _Project.Scripts.Features.GameScreen;
using _Tools;

namespace _Project.Scripts.Services
{
  public class Tasks
  {
    public Task GetAllPlayersLoadedEvent(GameLogic gameLogic)
    {
      Task clickEvent = Task.Delay(500);
        /*clickEvent = AsyncEvents.TaskFromEvent(
        h => gameLogic.OnAllPlayersLoaded += h,
        h => gameLogic.OnAllPlayersLoaded -= h);
        */
      
      return clickEvent;
    }
  }
}