using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.End_Interview.Scripts.Mono.Additional
{
  [RequireComponent(typeof(LayoutGroup))]
  public class UpdateLayoutGroup : MonoBehaviour
  {
    private LayoutGroup _group;

    private void OnEnable()
    {
      _group = GetComponent<LayoutGroup>();

      _group.enabled = false;
      _group.enabled = true;
    }

    private void Update()
    {
      _group.enabled = false;
      _group.enabled = true;
    }
  }
}