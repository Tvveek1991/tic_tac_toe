using System;
using UnityEngine;

namespace _Project.Scripts.MonoBehaviours.UI
{
  public class ObjectWithAnimation : MonoBehaviour
  {
    private Action _onDestroyAction;

    public void Construct(Action onDestroy) => 
      _onDestroyAction = onDestroy;

    private void OnDestroy() => 
      _onDestroyAction?.Invoke();
  }
}