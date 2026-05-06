using System;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Features.DragAndDrop.Serialized
{
  public static class TransformExtentions
  {
    public static void SetPlaceData(this Transform transform, PlaceData placeData)
    {
      transform.localPosition = placeData.Position;
      transform.localRotation = placeData.Rotation;
      transform.localScale = placeData.LocalScale;
    }

    public static void DoPlaceData(this Transform transform, PlaceData placeData, float duration, Action onEnd)
    {
      transform.DOLocalMove(placeData.Position, duration);
      transform.DOLocalRotate(placeData.Rotation.eulerAngles, duration).SetEase(Ease.OutExpo).onComplete += () => { onEnd?.Invoke(); };
      transform.DOScale(placeData.LocalScale, duration);
    }

    public static PlaceData GetPlaceData(this Transform transform)
    {
      return new()
      {
        Position = transform.localPosition,
        Rotation = transform.localRotation,
        LocalScale = transform.localScale,
      };
    }
  }
}