using UnityCoreModules.Services.EventBus;

namespace Match3.Scripts.Core.Events
{
    public struct ScoreUpdated: IEvent
    {
        public readonly int CurrentScore;
        public ScoreUpdated(int currentScore)
        {
            CurrentScore = currentScore;
        }
    }
}