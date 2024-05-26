using System;
using System.Collections.Generic;
using SableGamesRemote.Scripts.Data;

namespace SableGamesAds.Scripts.Data
{
  [Serializable]
  public struct Configurations
  {
    public PlacementType PlacementType;
    public List<RemotePair> RemotePair;
  }
}