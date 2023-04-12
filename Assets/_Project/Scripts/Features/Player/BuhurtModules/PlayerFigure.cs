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
    private readonly int _color = Shader.PropertyToID("_Color");
    
    public SymbolSize Size;
    
    private bool _isMoved;
    
    private MeshFilter _mesh;
    private MeshRenderer _meshRenderer;

    private DragableData _dragableData;
    private DragablePart _dragablePart;
    private PlayerData _playerData;

    private IDragDrop _dragDrop;

    public void Construct(IDragDrop dragDrop)
    {
      _mesh = GetComponentInChildren<MeshFilter>();
      _meshRenderer = GetComponentInChildren<MeshRenderer>();

      _dragDrop = dragDrop;
      
      _dragablePart = CreateDragableFromGameIconPrefab();
    }

    public void LoadFigure(PlayerData playerData, SymbolSize[] symbolSizes)
    {
      if (symbolSizes[(int) Size] == SymbolSize.Default) 
        _isMoved = true;
      
      _meshRenderer.enabled = !_isMoved;
      
      _playerData = playerData;

      _mesh.mesh = playerData.SymbolData.MeshBuhurt[(int)Size];
      _meshRenderer.material = playerData.SymbolData.MaterialBuhurt[(int)Size];
      _meshRenderer.material.SetColor(_color, playerData.Color);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
      if(_isMoved)
        return;
      
      _dragablePart.SetDragableData(new DragableData(_playerData.Sign, Size, _mesh.mesh, _meshRenderer.material));
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
