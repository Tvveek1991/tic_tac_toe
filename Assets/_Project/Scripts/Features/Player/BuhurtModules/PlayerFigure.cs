using System;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.DragAndDrop.Interface;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Services.GameMetaProvider;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.Features.Player.BuhurtModules
{
  public class PlayerFigure : MonoBehaviour, IPointerDownHandler
  {
    public SymbolSign Sign;
    public SymbolSize Size;

    private bool _isMoved;

    private MeshFilter _mesh;
    private MeshRenderer _meshRenderer;

    private DragableData _dragableData;
    private DragablePart _dragablePart;

    private IDragDrop _dragDrop;

    public void Construct(IDragDrop dragDrop)
    {
      _mesh = GetComponentInChildren<MeshFilter>();
      _meshRenderer = GetComponentInChildren<MeshRenderer>();

      _dragDrop = dragDrop;

      _dragablePart = CreateDragableFromGameIconPrefab();
    }

    public void LoadFigure(SymbolSign symbolSign, bool isMoved, Mesh mesh = null, Material material = null)
    {
      Sign = symbolSign;
      
      _isMoved = isMoved;
      _meshRenderer.enabled = !_isMoved;
      
      if(_isMoved)
        return;

      _mesh.mesh = mesh;
      _meshRenderer.material = material;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
      if (_isMoved)
        return;

      _dragablePart.SetDragableData(new DragableData(Sign, Size));
      _dragDrop.BindPartToCursor(_dragablePart);
    }

    private DragablePart CreateDragableFromGameIconPrefab()
    {
      var dragablePart = _mesh.gameObject.AddComponent<DragablePart>();
      dragablePart.Construct(OnStartDrag, OnPin, OnReturn);

      return dragablePart;
    }

    private void OnStartDrag()
    {
    }

    private void OnPin()
    {
    }

    private void OnReturn()
    {
    }
  }
}