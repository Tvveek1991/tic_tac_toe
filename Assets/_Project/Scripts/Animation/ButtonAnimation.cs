using System.Linq;
using _Project.Scripts.MonoBehaviours.UI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.Animation
{
  public class ButtonAnimation
  {
    private const float Duration = 0.2f;
    private const float TargetScale = 0.7f;

    private readonly Transform _target;
    
    private readonly Vector3 _originalScale;
    private Tween _scale;
    private EventTrigger _eventTrigger;
    
    private EventTrigger.Entry _entryUp;
    private EventTrigger.Entry _entryDown;
    
    private readonly CanvasGroup[] _parentGroups;

    public ButtonAnimation(Transform target)
    {
      var objectWithAnimation = target.gameObject.AddComponent<ObjectWithAnimation>();
      objectWithAnimation.Construct(() => _scale?.Kill());
      
      _target = target;
      _originalScale = _target.localScale;
      _parentGroups = _target.GetComponentsInParent<CanvasGroup>();

      InitializeTriggers(_target);
    }

    private void ButtonDown()
    {
      if(!IsButtonInteractable())
        return;
      
      _scale?.Kill();
      _scale = _target.DOScale(TargetScale * _originalScale, Duration).SetEase(Ease.Flash);
    }

    private void ButtonUp()
    {
      if(!IsButtonInteractable())
        return;

      _scale?.Kill();
      _scale = _target.DOScale(_originalScale, Duration).SetEase(Ease.Flash);
    }

    private void InitializeTriggers(Transform target)
    {
      _eventTrigger = target.gameObject.AddComponent<EventTrigger>();

      _entryUp = new EventTrigger.Entry();
      _entryUp.eventID = EventTriggerType.PointerUp;
      _entryUp.callback.AddListener(arg => ButtonUp());

      _entryDown = new EventTrigger.Entry();
      _entryDown.eventID = EventTriggerType.PointerDown;
      _entryDown.callback.AddListener(arg => ButtonDown());

      _eventTrigger.triggers.Add(_entryUp);
      _eventTrigger.triggers.Add(_entryDown);
    }

    private bool IsButtonInteractable() => 
      _parentGroups.All(p => p.interactable);
  }
}