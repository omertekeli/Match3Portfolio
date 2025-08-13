using System;
using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Level.Base;
using Match3.Scripts.Systems.Level.Data;

namespace Match3.Scripts.Systems.Level.Goals
{
    public class GemMatchGoal : LevelGoalBase
    {
        #region Properties
        public GemType GoalGemType { get; private set; }
        public int TargetCount { get; private set; }
        public int CurrentCount { get; private set; }
        #endregion

        public event Action<int, int> OnProgressUpdated;

        public GemMatchGoal(GemGoalData goalData)
        {
            GoalGemType = goalData.GoalGemType;
            TargetCount = goalData.TargetCount;
        }

        public bool ProcessClearedPiece(GemType type, int count)
        {
            if (IsCompleted || type != GoalGemType)
            {
                return false;
            }

            CurrentCount += count;
            if (CurrentCount >= TargetCount)
            {
                CurrentCount = TargetCount;
                base.CompleteGoal();
            }

            OnProgressUpdated?.Invoke(CurrentCount, TargetCount);
            return true;
        }
    }
}