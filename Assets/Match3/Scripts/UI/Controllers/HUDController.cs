using System.Collections.Generic;
using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Level.Base;
using Match3.Scripts.Systems.Level.Data;
using Match3.Scripts.UI.Base;
using Match3.Scripts.UI.Views;

namespace Match3.Scripts.UI.Controllers
{
    public class HUDController : UIController<HUDView>
    {
        #region Fields

        private GoalPanelController _goalPanelController;

        #endregion
        public HUDController(HUDView view) : base(view)
        {
            _goalPanelController = new GoalPanelController(view.GoalPanelView);
        }

        internal void ToggleHUD(bool shouldShow) => View.SetVisible(shouldShow);
        internal void SetupUI(LevelDataSO leveldata, IReadOnlyList<LevelGoalBase> goals)
        {
            UpdateRemainingMove(leveldata.MaxMove);
            UpdateScore(0);
            _goalPanelController.SetupGoals(goals);
        }

        internal void UpdateRemainingMove(int remainingMove) => View.UpdateRemainingMove(remainingMove);
        internal void UpdateScore(int score) => View.UpdateScore(score);
        internal void UpdateProgressBar(float totalProgress) => View.UpdateProgressBar(totalProgress);
        internal void UpdateGoals(Dictionary<GemType, int> clearedPieces) => _goalPanelController.UpdateGoals(clearedPieces);
    }
}