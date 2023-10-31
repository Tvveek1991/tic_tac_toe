using System;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.DragAndDrop.Interface;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.Features.Player.BuhurtModules
{
  public class PlayerFigure : MonoBehaviour, IPointerDownHandler
  {
    private readonly int _color = Shader.PropertyToID("_Color");
    
    [SerializeField] private SymbolSize _size;

    private BoxCollider _boxCollider;

    private MeshFilter _mesh;
    private MeshRenderer _meshRenderer;

    private DragableData _dragableData;
    private DragablePart _dragablePart;

    private IDragDrop _dragDrop;

    private IInputResolver _inputResolver;

    private bool _isLock;

    public SymbolSign Sign { get; private set; }
    public SymbolSize Size => _size;

    private Action _hidePotentials;
    private Action<DragablePart> _showPotentialTurn;

    public void Construct(IDragDrop dragDrop, IInputResolver inputResolver, Action<DragablePart> showPotentialTurn, Action hidePotentials)
    {
      _mesh = GetComponentInChildren<MeshFilter>();
      _meshRenderer = GetComponentInChildren<MeshRenderer>();
      _boxCollider = GetComponentInChildren<BoxCollider>();

      _hidePotentials = hidePotentials;
      _showPotentialTurn = showPotentialTurn;
      
      _dragDrop = dragDrop;
      _inputResolver = inputResolver;

      _dragablePart = CreateDragableFromGameIconPrefab();
    }

    public void FirstLoadFigure(SymbolSign symbolSign, Mesh mesh, Material material, Vector3 angle)
    {
      Sign = symbolSign;

      _mesh.mesh = mesh;
      _meshRenderer.material = material;
      
      _mesh.transform.localRotation = Quaternion.Euler(angle);
    }

    public void SetVisibility(bool isOn)
    {
      if (_size == SymbolSize.Default)
      {
        _meshRenderer.enabled = false;
        return;
      }

      _isLock = !isOn;

      Color color = _meshRenderer.material.GetColor(_color);
      color.a = isOn ? 1 : .4f;
      _meshRenderer.material.SetColor(_color, color);
    }

    public void SetSizeToMoved() => 
      _size = SymbolSize.Default;

    public void OnPointerDown(PointerEventData eventData)
    {
      if(_dragDrop == null)
        return;
      
      if (_size == SymbolSize.Default || _inputResolver.IsLocked)
        return;

      if (_isLock)
        return;

      _dragablePart.SetDragableData(new DragableData(Sign, _size));
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
      _boxCollider.enabled = false;
      _showPotentialTurn.Invoke(_dragablePart);
    }

    private void OnPin() { }

    private void OnReturn()
    {
      _boxCollider.enabled = true;
      _hidePotentials?.Invoke();
    }
  }
}