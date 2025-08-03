using System.Collections.Generic;
using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Level.Base;
using Match3.Scripts.Systems.Level.Data;
using Match3.Scripts.UI.Base;
using Match3.Scripts.UI.Views;

namespace Match3.Scripts.UI.Controllers
{
    public class HUDController: UIController<HUDView>
    {
        #region Fields

        private GoalPanelController _goalPanelController;

        #endregion
        public HUDController(HUDView view) : base(view)
        {
            _goalPanelController = new GoalPanelController(view.GoalPanelView);
        }

        internal void ToggleHUD(GameState state)
        {
            bool shouldShow = state != GameState.MainMenu && state != GameState.Loading;
            View.SetVisible(shouldShow);
        }
        internal void SetupUI(LevelDataSO leveldata, IReadOnlyList<LevelGoalBase> goals)
        {
            View.UpdateRemainingMove(leveldata.MaxMove);
            View.UpdateScore(0);
            _goalPanelController.SetupGoals(goals);
        }
    }
}