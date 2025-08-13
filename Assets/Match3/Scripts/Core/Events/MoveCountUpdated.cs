using UnityCoreModules.Services.EventBus;

namespace Match3.Scripts.Core.Events
{
    public struct MoveCountUpdated: IEvent
    {
        public readonly int RemaningMove;
        public MoveCountUpdated(int remainigMove)
        {
            RemaningMove = remainigMove;
        }
    }
}