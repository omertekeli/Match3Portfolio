using System;
using Match3.Scripts.Enums;
using Match3.Scripts.LevelSystem.Data;

namespace Match3.Scripts.LevelSystem.Goals
{
    public class GemMatchGoal: LevelGoalBase
    {
        #region Properties
        public GemType GoalGemType { get; private set; }
        public int TargetCount { get; private set; }
        public int CurrentCount{ get; private set; }
        
        #endregion

        
        public event Action<int, int> OnProgressUpdated;
                
        public GemMatchGoal(GemGoalData goalData)
        {
            GoalGemType = goalData.GoalGemType;
            TargetCount = goalData.TargetCount;
        }
        
        public void TryToMatch(GemType gemType)
        {
            if (IsCompleted || gemType != GoalGemType) return;
            CurrentCount++;
            OnProgressUpdated?.Invoke(CurrentCount, TargetCount);
            if (CurrentCount >= TargetCount)
            {
                base.CompleteGoal();
            }
        }
    }
}