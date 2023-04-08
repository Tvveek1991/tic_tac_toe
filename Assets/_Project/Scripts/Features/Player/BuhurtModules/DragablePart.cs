using System;
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

    private SymbolSize _size;

    private MeshFilter _mesh;
    private MeshRenderer _meshRenderer;

    private Action _onPin;
    private Action _onReturn;
    private Action _onStartDrag;

    public bool IsPinned { get; private set; }

    public Transform PinPoint;

    public void Construct(Mesh mesh, Material material, SymbolSize size, Action onStartDrag, Action onPin, Action onReturn)
    {
      _mesh = GetComponentInChildren<MeshFilter>();
      _meshRenderer = GetComponentInChildren<MeshRenderer>();
      
      _originalPlaceData = GetPlaceData();
      
      _size = size;
      _mesh.mesh = mesh;
      _meshRenderer.material = material;

      _onPin = onPin;
      _onReturn = onReturn;
      _onStartDrag = onStartDrag;
    }

    public void ToGameWorld()
    {
      _onStartDrag?.Invoke();
      _baseParent = transform.parent;
      transform.SetParent(_mesh.transform.parent);
      SetPlaceData(_mesh.transform.GetPlaceData());
    }

    public void ReturnToBaseState()
    {
      IsPinned = false;
      transform.SetParent(_baseParent);
      SetPlaceData(_originalPlaceData);

      _onReturn?.Invoke();
    }

    public void PinPart()
    {
      PinToPlace();
    }

    public bool CanPin()
    {
      return false;
    }

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
      Destroy(gameObject);
    }
  }
}