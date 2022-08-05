using System.Linq;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.Services.Audio
{
  public class ButtonSound
  {
    private PointerClickTrigger _eventTrigger;
    private EventTrigger.Entry _entryDown;

    private readonly CanvasGroup[] _parentGroups;

    private readonly IAudioService _audioService;
    private readonly AudioId _audioId;

    public ButtonSound(IAudioService audioService, GameObject gameObject, AudioId audioId)
    {
      _parentGroups = gameObject.GetComponentsInParent<CanvasGroup>();

      _audioService = audioService;
      _audioId = audioId;

      InitializeTriggers(gameObject);
    }

    private void ButtonDown()
    {
      if (!IsButtonInteractable())
        return;

      _audioService.Play(_audioId);
    }

    private void InitializeTriggers(GameObject gameObject)
    {
      _eventTrigger = gameObject.AddComponent<PointerClickTrigger>();

      _entryDown = new EventTrigger.Entry();
      _entryDown.eventID = EventTriggerType.PointerClick;
      _entryDown.callback.AddListener(arg => ButtonDown());

      _eventTrigger.triggers.Add(_entryDown);
    }

    private bool IsButtonInteractable() => 
      _parentGroups.All(p => p.interactable);
  }
}