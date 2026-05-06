using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.Services.Audio.Executor
{
    public class PointerClickTrigger : MonoBehaviour, IPointerClickHandler
    {
        public List<EventTrigger.Entry> triggers = new List<EventTrigger.Entry>();

        public void OnPointerClick(PointerEventData eventData)
        {
            Execute(EventTriggerType.PointerClick, eventData);
        }

        private void Execute(EventTriggerType id, BaseEventData eventData)
        {
            foreach (var trigger in triggers)
            {
                if (trigger.eventID == id && trigger.callback != null)
                    trigger.callback.Invoke(eventData);
            }
        }
    }
}