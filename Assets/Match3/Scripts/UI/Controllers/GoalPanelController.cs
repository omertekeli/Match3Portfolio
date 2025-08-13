using System.Collections.Generic;
using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Level.Base;
using Match3.Scripts.UI.Base;
using Match3.Scripts.UI.Views;

namespace Match3.Scripts.UI.Controllers
{
    public class GoalPanelController : UIController<GoalPanelView>
    {
        public GoalPanelController(GoalPanelView view) : base(view) { }

        internal void SetupGoals(IReadOnlyList<LevelGoalBase> goals)
        {
            View.SetGoals(goals);
        }

        internal void UpdateGoals(Dictionary<GemType, int> clearedPieces)
        {
            View.UpdateGoals(clearedPieces);
        }
    }
}