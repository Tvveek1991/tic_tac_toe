using System;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.DragAndDrop.Interface;
using _Project.Scripts.Features.DragAndDrop.Serialized;
using _Project.Scripts.Features.GameScreen.Serialized;
using UnityEngine;

namespace _Project.Scripts.Features.Player.BuhurtModules
{
  public class DragablePart : MonoBehaviour
  {
    private Transform _baseParent;
    private PlaceData _originalPlaceData;

    private IDragDrop _dragDrop;

    private DragableData _dragableData;

    private Action _onPin;
    private Action _onReturn;

    private Action _onStartDrag;

    public void Construct(Action onStartDrag, Action onPin, Action onReturn)
    {
      _onPin = onPin;
      _onReturn = onReturn;
      _onStartDrag = onStartDrag;
    }

    public void ToGameWorld()
    {
      _originalPlaceData = GetPlaceData();
      
      _onStartDrag?.Invoke();
      _baseParent = transform.parent;
      transform.SetParent(transform.parent);
      SetPlaceData(transform.GetPlaceData());
    }

    public void ReturnToBaseState()
    {
      transform.SetParent(_baseParent);
      
      SetPlaceData(_originalPlaceData);
      _onReturn?.Invoke();
    }

    public void SetDragableData(DragableData dragableData) =>
      _dragableData = dragableData;

    public DragableData GetDragableData() =>
      _dragableData;

    public void PinPart() => 
      _onPin?.Invoke();

    public bool CanPin(SymbolSize symbolSize, SymbolSign symbolSign) => 
      symbolSize < _dragableData.GetSize() && symbolSign != _dragableData.GetSymbol();

    private void SetPlaceData(PlaceData placeData) =>
      transform.SetPlaceData(placeData);

    private PlaceData GetPlaceData()
    {
      Transform partTransform = transform;
      _originalPlaceData = partTransform.GetPlaceData();

      return _originalPlaceData;
    }
  }
}