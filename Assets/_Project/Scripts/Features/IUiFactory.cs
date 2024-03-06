using _Project.Scripts.Services.Money.Mono;

namespace _Project.Scripts.Features
{
  public interface IUiFactory
  {
    // LoadWindow Loading { get; }
    Settings.Settings SettingsWindow { get; }
    NotLoaded.NotLoadedWindow NoInternetWindow { get; }
    // LoadWindow CreateLoadWindow();

    MoneyCounter SpawnMoneyCounter();
    void CreateSettingsView();
    void SpawnNoInternetConnection();
    void ClearLoadWindow();
    void ClearSettingsWindow();
    void ClearNoInternetWindow();
  }
}