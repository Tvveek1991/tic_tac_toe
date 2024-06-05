using System;

namespace SableGamesAnalytics.Scripts.Logger
{
  public class SableGamesAnalyticsException : Exception
  {
    public SableGamesAnalyticsException(string toString) : base(toString)
    {}
  }
}