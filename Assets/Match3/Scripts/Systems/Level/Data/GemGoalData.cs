using System;
using Match3.Scripts.Enums;

namespace Match3.Scripts.LevelSystem.Data
{
    [Serializable]
    public struct GemGoalData
    {
        public GemType GoalGemType;
        public int TargetCount;
    }
}