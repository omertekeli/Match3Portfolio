using System;
using Match3.Scripts.Core.Events;

namespace Match3.Scripts.Core.Interfaces
{
    public interface IInputSystem: IService
    {
        event Action<SwapRequested> SwapRequested;
    }
}