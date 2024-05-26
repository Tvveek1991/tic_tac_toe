using System;

namespace SableGamesAds.Scripts.Logger
{
  public class SableGamesAdsException : Exception
  {
    public SableGamesAdsException(string toString) : base(toString)
    {}
  }
}