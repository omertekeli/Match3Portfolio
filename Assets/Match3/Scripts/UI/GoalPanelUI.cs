using Match3.Scripts.LevelSystem.Goals;
using UnityEngine;

namespace Match3.Scripts.UI
{
    public class GoalPanelUI: MonoBehaviour
    {
        [SerializeField] private GoalSlotUIBase[] _slots;

        public void SetGoals(LevelGoalBase[] goals)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                _slots[i].Set(goals[i]); 
            }
        }
    }
}