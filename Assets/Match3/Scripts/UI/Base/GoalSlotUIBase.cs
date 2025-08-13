using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Level.Base;
using UnityEngine;

namespace Match3.Scripts.UI.Base
{
    public abstract class GoalSlotUIBase : MonoBehaviour
    {
        protected internal abstract void Set(LevelGoalBase goal);
        protected internal virtual void UpdateCount(int collectedCount) { }
        protected internal virtual void UpdateCount(GemType gemType, int collectedCount) { }
    }
}