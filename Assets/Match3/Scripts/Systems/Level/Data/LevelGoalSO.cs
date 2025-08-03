using UnityEngine;
using Match3.Scripts.Systems.Level.Base;

namespace Match3.Scripts.Systems.Level.Data
{
    public abstract class LevelGoalSO: ScriptableObject
    {
        public abstract LevelGoalBase CreateGoal();
    }
}