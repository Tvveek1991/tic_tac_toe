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

    private Vector3 _smoothDampVelocity;
    private Coroutine _follow;

    private bool _isInDrag;

    public DragablePart CurrentActiveDragPart { get; private set; }

    private Vector3 PartPosition
    {
      set
      {
        value.z = -5;
        CurrentActiveDragPart.transform.position = value;
      }
      get => CurrentActiveDragPart.transform.position;
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

        yield return waitForEndOfFrame;
      }

      ResetAfterRelease();
    }

    private void ResetAfterRelease()
    {
      _isInDrag = false;
      CurrentActiveDragPart = null;
    }
  }
}