using Match3.Scripts.LevelSystem.Goals;
using UnityEngine;

namespace Match3.Scripts.UI.Base
{
    public abstract class GoalSlotUIBase: MonoBehaviour
    {
        protected internal abstract void Set(LevelGoalBase goal);
    }
}