using System;
using System.Collections.Generic;
using Match3.Scripts.LevelSystem.Goals;
using Match3.Scripts.UI.Base;
using Match3.Scripts.UI.Views;

namespace Match3.Scripts.UI.Controllers
{
    public class GoalPanelController : UIController<GoalPanelView>
    {
        public GoalPanelController(GoalPanelView view) : base(view) {}

        internal void SetupGoals(List<LevelGoalBase> goals)
        {
            View.SetGoals(goals);
        }
    }
}