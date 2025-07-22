using Match3.Scripts.LevelSystem.Goals;
using UnityEngine;

namespace Match3.Scripts.LevelSystem.Data
{
    [CreateAssetMenu(menuName = "Match3/Gem Match Goal")]
    public class GemMatchGoalSO: LevelGoalSO
    {
        [SerializeField] private GemGoalData _goalData;
        
        public override LevelGoalBase CreateGoal()
        {
            return new GemMatchGoal(_goalData);
        }
    }
}