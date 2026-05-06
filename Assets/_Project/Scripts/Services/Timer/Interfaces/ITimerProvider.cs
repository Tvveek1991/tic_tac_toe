using System;

namespace _Project.Scripts.Services.Timer.Interfaces
{
    public interface ITimeProvider
    {
        DateTime GetCurrentTime();
    }
}
