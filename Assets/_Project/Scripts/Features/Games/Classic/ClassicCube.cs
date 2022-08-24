using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.Features.Games.Classic
{
  public class ClassicCube : MonoBehaviour, IPointerClickHandler
  {
    public void OnPointerClick(PointerEventData eventData)
    {
      Debug.Log(gameObject.name);
    }
  }
}