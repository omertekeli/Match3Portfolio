using System;

namespace Match3.Scripts.Systems.Level.Base
{
    public abstract class LevelGoalBase
    {
        public bool IsCompleted { get; private set; }
        public event Action OnGoalCompleted;
        
        protected void CompleteGoal()
        {
            if (IsCompleted) return;
            IsCompleted = true;
            OnGoalCompleted?.Invoke();
        }
    }
}