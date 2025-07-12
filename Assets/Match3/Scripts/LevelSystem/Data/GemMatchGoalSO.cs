using Match3.Scripts.Enums;
using Match3.Scripts.LevelSystem.Goals;
using UnityEngine;

namespace Match3.Scripts.LevelSystem.Data
{
    [CreateAssetMenu(menuName = "Match3/Gem Match Goal")]
    public class GemMatchGoalSO: LevelGoalSO
    {
        [SerializeField] private GemType _gemType;
        [SerializeField] private int _targetCount;
        
        public override LevelGoalBase CreateGoal()
        {
            return new GemMatchGoal(_gemType, _targetCount);
        }
    }
}