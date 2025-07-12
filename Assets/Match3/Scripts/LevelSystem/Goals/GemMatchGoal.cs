using System;
using Match3.Scripts.Enums;

namespace Match3.Scripts.LevelSystem.Goals
{
    public class GemMatchGoal: LevelGoalBase
    {
        private GemType _gemType;
        private int _targetCount;
        private int _currentCount = 0;
        
        public event Action<int, int> OnProgressUpdated;
                
        public GemMatchGoal(GemType gemType, int targetCount)
        {
            _gemType = gemType;
            _targetCount = targetCount;
        }
        
        public void TryToMatch(GemType gemType)
        {
            if (IsCompleted || gemType != _gemType) return;
            _currentCount++;
            OnProgressUpdated?.Invoke(_currentCount, _targetCount);
            if (_currentCount >= _targetCount)
            {
                base.CompleteGoal();
            }
        }
    }
}