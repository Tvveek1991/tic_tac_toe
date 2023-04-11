using System;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.DragAndDrop.Interface;
using _Project.Scripts.Features.DragAndDrop.Serialized;
using _Project.Scripts.Features.GameScreen.Serialized;
using Unity.Netcode;
using UnityEngine;

namespace _Project.Scripts.Features.Player.BuhurtModules
{
  public class DragablePart : MonoBehaviour
  {
    private Transform _baseParent;
    private PlaceData _originalPlaceData;

    private IDragDrop _dragDrop;

    private DragableData _dragableData;
    
    /*
    private SymbolSize _size;
    private MeshFilter _mesh;
    private MeshRenderer _meshRenderer;*/

    private Action _onPin;
    private Action _onReturn;

    private Action _onStartDrag;

    public Transform PinPoint;
    
    public bool IsPinned { get; private set; }

    public void Construct(Action onStartDrag, Action onPin, Action onReturn)
    {
      _originalPlaceData = GetPlaceData();

      _onPin = onPin;
      _onReturn = onReturn;
      _onStartDrag = onStartDrag;
    }

    public void ToGameWorld()
    {
      _onStartDrag?.Invoke();
      _baseParent = transform.parent;
      transform.SetParent(transform.parent);
      SetPlaceData(transform.GetPlaceData());
    }

    public void ReturnToBaseState()
    {
      IsPinned = false;
      transform.SetParent(_baseParent);
      SetPlaceData(_originalPlaceData);

      _onReturn?.Invoke();
    }

    public void SetDragableData(DragableData dragableData) =>
      _dragableData = dragableData;

    public DragableData GetDragableData() =>
      _dragableData;

    public void PinPart()
    {
      PinToPlace();
    }

    public bool CanPin(SymbolSize symbolSize, SymbolSign netSymbolSign) => 
      symbolSize < _dragableData.GetSize() && netSymbolSign != _dragableData.GetSymbol();

    private void SetPlaceData(PlaceData placeData) =>
      transform.SetPlaceData(placeData);

    private PlaceData GetPlaceData()
    {
      Transform partTransform = transform;
      _originalPlaceData = partTransform.GetPlaceData();

      return _originalPlaceData;
    }

    private void PinToPlace()
    {
      IsPinned = true;
      OnEndPin(false);
    }

    private void OnEndPin(bool shouldExecuteCallback = true)
    {
      /*var renderer = GetComponentInChildren<Renderer>(true);
      GamePart.GetComponent<PartMaterial>().SetMaterial(renderer.sharedMaterial);
      GamePart.IsPinned = true;

      OnOffStateAnimation offStateAnimation = new OnOffStateAnimation(UIPart.transform);
      offStateAnimation.Disable(() => UIPart.gameObject.SetActive(false));

      if (shouldExecuteCallback)
        OnPin?.Invoke(GamePart);*/

      Destroy(gameObject);
    }

    private void OnMouseUp()
    {
      Debug.Log("UP DRAGABLE PART");
      _onReturn?.Invoke();
      // Destroy(gameObject);
    }
  }
}