using SableGamesAds.Scripts.Data;

namespace SableGamesAds.Scripts.Logger
{
  public interface IAdsLogger
  {
    void ThrowNotSetupException();
    void SetSettings(SableGamesAdsSettings adsSettings);
    void LogWithoutFormat(string s);
  }
}