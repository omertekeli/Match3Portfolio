using System.Collections.Generic;
using System.Linq;
using Match3.Scripts.Core.Events;
using Match3.Scripts.Systems.Board.Events;
using Match3.Scripts.Systems.Level.Base;
using Match3.Scripts.Systems.Level.Goals;
using UnityCoreModules.Services;
using UnityCoreModules.Services.EventBus;
using UnityEngine;

namespace Match3.Scripts.Core
{
    public class GoalSystem : IService
    {
        #region Fields
        private readonly IEventBus _eventBus;
        private List<LevelGoalBase> _activeGoals;
        #endregion

        public GoalSystem(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<PiecesCleared>(OnPiecesCleared);
        }

        #region Methods
        public void Initialize(List<LevelGoalBase> goals)
        {
            _activeGoals = goals;
        }

        public bool IsAllGoalsCompleted()
        {
            return _activeGoals.All(g => g.IsCompleted);
        }

        public void Shutdown()
        {
            _eventBus.Unsubscribe<PiecesCleared>(OnPiecesCleared);
        }

        private void OnPiecesCleared(PiecesCleared eventData)
        {
            if (_activeGoals == null || _activeGoals.Count == 0 || IsAllGoalsCompleted())
                return;

            bool wasGoalUpdated = false;
            foreach (var goal in _activeGoals)
            {
                if (goal is GemMatchGoal gemGoal)
                {
                    if (eventData.ClearedPieces.TryGetValue(gemGoal.GoalGemType, out int count))
                    {
                        if (gemGoal.ProcessClearedPiece(gemGoal.GoalGemType, count))
                        {
                            wasGoalUpdated = true;
                        }
                    }
                }
            }

            if (wasGoalUpdated)
            {
                float totalProgress = CalculateTotalProgress();
                _eventBus.Fire(new GoalUpdated(totalProgress));
            }

            if (IsAllGoalsCompleted())
            {
                _eventBus.Fire(new LevelEnd(true));
                Debug.Log("LEVEL WON!");
            }
        }

        private float CalculateTotalProgress()
        {
            if (_activeGoals == null || _activeGoals.Count == 0)
                return 0f;

            float totalTarget = 0;
            float totalCurrent = 0;

            foreach (var goal in _activeGoals)
            {
                if (goal is GemMatchGoal gemGoal)
                {
                    totalTarget += gemGoal.TargetCount;
                    totalCurrent += gemGoal.CurrentCount;
                }
            }

            if (totalTarget == 0)
                return 0f;
            return totalCurrent / totalTarget;
        }
    }
    #endregion
}