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
    public SymbolSign Sign;
    public SymbolSize Size;

    private bool _isMoved;

    private BoxCollider _boxCollider;
    
    private MeshFilter _mesh;
    private MeshRenderer _meshRenderer;

    private DragableData _dragableData;
    private DragablePart _dragablePart;

    private IDragDrop _dragDrop;
    
    private IInputResolver _inputResolver;

    public void Construct(IDragDrop dragDrop, IInputResolver inputResolver)
    {
      _mesh = GetComponentInChildren<MeshFilter>();
      _meshRenderer = GetComponentInChildren<MeshRenderer>();
      _boxCollider = GetComponentInChildren<BoxCollider>();

      _dragDrop = dragDrop;
      _inputResolver = inputResolver;

      _dragablePart = CreateDragableFromGameIconPrefab();
    }

    public void LoadFigure(SymbolSign symbolSign, bool isMoved, Mesh mesh = null, Material material = null)
    {
      Sign = symbolSign;
      
      _isMoved = isMoved;
      SetVisibility(!_isMoved);
      
      if(_isMoved)
        return;

      _mesh.mesh = mesh;
      _meshRenderer.material = material;
    }

    public void SetVisibility(bool isOn) => 
      _meshRenderer.enabled = isOn;

    public void OnPointerDown(PointerEventData eventData)
    {
      if (_isMoved || _inputResolver.IsLocked)
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
      _boxCollider.enabled = false;
    }

    private void OnPin()
    {
    }

    private void OnReturn()
    {
      _boxCollider.enabled = true;
    }
  }
}