using Match3.Scripts.Systems.Level.Data;
using UnityCoreModules.Services.EventBus;

namespace Match3.Scripts.Core.Events
{
    public struct LevelLoaded : IEvent
    {
        public readonly int LevelIndex;
        public readonly LevelDataSO LevelData;
        public LevelLoaded( int levelIndex, LevelDataSO levelDataSO)
        {
            LevelIndex = levelIndex;
            LevelData = levelDataSO;
        }
    }
}