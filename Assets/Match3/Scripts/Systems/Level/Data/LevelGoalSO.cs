using UnityEngine;
using Match3.Scripts.LevelSystem.Goals;

namespace Match3.Scripts.LevelSystem.Data
{
    public abstract class LevelGoalSO: ScriptableObject
    {
        public abstract LevelGoalBase CreateGoal();
    }
}