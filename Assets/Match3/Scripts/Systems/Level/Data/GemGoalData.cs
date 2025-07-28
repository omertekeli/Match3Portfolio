using System;
using Match3.Scripts.Enums;

namespace Match3.Scripts.Systems.Level.Data
{
    [Serializable]
    public struct GemGoalData
    {
        public GemType GoalGemType;
        public int TargetCount;
    }
}