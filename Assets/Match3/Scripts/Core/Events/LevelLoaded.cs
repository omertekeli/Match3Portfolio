using UnityCoreModules.Services.EventBus;

namespace Match3.Scripts.Core.Events
{
    public struct LevelLoaded : IEvent
    {
        public readonly int LevelIndex;
        public LevelLoaded(int levelIndex)
        {
            LevelIndex = levelIndex;
        }
    }
}