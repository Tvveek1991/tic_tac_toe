using System.Collections;
using _Project.Scripts.Features.DragAndDrop.Interface;
using _Project.Scripts.Features.Player.BuhurtModules;
using UnityEngine;

namespace _Project.Scripts.Features.DragAndDrop
{
  public class DragDrop : MonoBehaviour, IDragDrop
  {
    private const float DragCameraOffset = 20f;
    private const float SmoothTime = 0.1f;

    private Camera _camera;

    private DragablePart CurrentActiveDragPart { get; set; }

    private Vector3 _smoothDampVelocity;
    private Coroutine _follow;
    
    private bool _isInDrag;

    private Vector3 PartPosition
    {
      set
      {
        value.z = 0;
        if (CurrentActiveDragPart.PinPoint == null)
          CurrentActiveDragPart.transform.position = value;
        else
        {
          Vector3 delta = CurrentActiveDragPart.transform.position - CurrentActiveDragPart.PinPoint.position;
          CurrentActiveDragPart.transform.position = value + delta; 
        }
      }
      get => CurrentActiveDragPart.PinPoint == null ?
        CurrentActiveDragPart.transform.position :
        CurrentActiveDragPart.PinPoint.position;
    }

    private void Start() =>
      _camera = Camera.main;

    private void LateUpdate()
    {
      if (Input.GetMouseButtonUp(0))
        ReleasePart();
    }

    public void BindPartToCursor(DragablePart dragablePart)
    {
      dragablePart.ToGameWorld();

      CurrentActiveDragPart = dragablePart;
      
      _isInDrag = true;
      _follow = StartCoroutine(FollowCursor());
    }

    private void ReleasePart()
    {
      if (_follow != null)
      {
        StopCoroutine(_follow);
        _follow = null;
      }

      if (CurrentActiveDragPart == null)
        return;

      if (CurrentActiveDragPart.IsPinned) 
        return;
      
      CurrentActiveDragPart.ReturnToBaseState();
      ResetAfterRelease();
    }

    private IEnumerator FollowCursor()
    {        
      var waitForEndOfFrame = new WaitForEndOfFrame();
      
      Vector3 targetPoint = Input.mousePosition;
      targetPoint = _camera.ScreenPointToRay(targetPoint).origin;
      targetPoint += _camera.transform.forward * DragCameraOffset;      
      
      PartPosition = targetPoint;
        
      while (_isInDrag)
      {
        targetPoint = Input.mousePosition;
        targetPoint = _camera.ScreenPointToRay(targetPoint).origin;
        targetPoint += _camera.transform.forward * DragCameraOffset;

        if (CurrentActiveDragPart == null)
        {
          _isInDrag = false;
          yield break;
        }

        PartPosition = Vector3.SmoothDamp(PartPosition, targetPoint, ref _smoothDampVelocity, SmoothTime);
        Debug.Log(PartPosition);
          
        yield return waitForEndOfFrame;

        if (!CurrentActiveDragPart.CanPin() || CurrentActiveDragPart.IsPinned)
          continue;

        CurrentActiveDragPart.PinPart();
        ResetAfterRelease();
      }
    }

    private void ResetAfterRelease()
    {
      _isInDrag = false;
      CurrentActiveDragPart = null;
    }
  }
}