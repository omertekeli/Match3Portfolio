using System.Collections.Generic;
using Match3.Scripts.LevelSystem.Goals;
using Match3.Scripts.UI.Base;
using UnityEngine;

namespace Match3.Scripts.UI.Views
{
    public class GoalPanelView : UIView
    {
        #region Fields
        [SerializeField] private Transform _gemSlotContainer;
        [SerializeField] private GoalSlotUIBase _gemGoalSlotPrefab;

        private readonly List<GoalSlotUIBase> _spawnedSlots = new();

        #endregion

        internal void SetGoals(List<LevelGoalBase> goals)
        {
            ClearSlots();
            foreach (var goal in goals)
            {
                GoalSlotUIBase prefabToUse = goal switch
                {
                    GemMatchGoal => _gemGoalSlotPrefab,
                    _ => null
                };

                if (!prefabToUse) continue;

                var slot = Instantiate(prefabToUse, _gemSlotContainer);
                slot.Set(goal);
                _spawnedSlots.Add(slot);
            }
        }

        private void ClearSlots()
        {
            if (_spawnedSlots.Count < 1) return;
            foreach (var slot in _spawnedSlots)
            {
                if (slot) Destroy(slot.gameObject);
            }
            _spawnedSlots.Clear();
        }
    }
}