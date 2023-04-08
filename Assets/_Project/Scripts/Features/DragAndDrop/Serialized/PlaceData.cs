using System;
using UnityEngine;

namespace _Project.Scripts.Features.DragAndDrop.Serialized
{
  [Serializable]
  public struct PlaceData
  {
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 LocalScale;
  }
}