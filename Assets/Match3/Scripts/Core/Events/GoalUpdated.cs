using UnityCoreModules.Services.EventBus;

namespace Match3.Scripts.Core.Events
{
    public struct GoalUpdated : IEvent
    {
        public readonly float TotalProgress;

        public GoalUpdated(float totalProgress)
        {
            TotalProgress = totalProgress;
        }
    }
}