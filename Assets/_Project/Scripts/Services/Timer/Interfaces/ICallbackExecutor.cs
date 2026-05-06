using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Timer.Interfaces
{
    public interface ICallbackExecutor
    {
        string GetTimerGuid();
        void ExecuteWhenTick(int seconds);
        void ExecuteWhenComplete();
        void SetLock(bool isLocked);
        void Destroy();
    }
}
