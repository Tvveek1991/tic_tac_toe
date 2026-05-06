using System;

namespace SableGamesRemote.Scripts.Logger
{
  public class SableGamesRemoteException : Exception
  {
    public SableGamesRemoteException(string toString) : base(toString)
    {}
  }
}