using Match3.Scripts.Systems.Level.Base;
using Match3.Scripts.Systems.Level.Goals;
using UnityEngine;

namespace Match3.Scripts.Systems.Level.Data
{
    [CreateAssetMenu(fileName = "GemMatch_", menuName = "Match3/Gem Match Goal")]
    public class GemMatchGoalSO: LevelGoalSO
    {
        [SerializeField] private GemGoalData _goalData;
        
        public override LevelGoalBase CreateGoal()
        {
            return new GemMatchGoal(_goalData);
        }
    }
}