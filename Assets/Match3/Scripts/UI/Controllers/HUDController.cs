using System;
using System.Collections.Generic;
using Match3.Scripts.Core;
using Match3.Scripts.Core.Interfaces;
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
        private UIManager _uiManager;
        private IAudioManager _audioManager;
        #endregion

        #region Actions

        #endregion

        public HUDController(HUDView view, UIManager uiManager, IAudioManager audioManager) : base(view)
        {
            _uiManager = uiManager;
            _audioManager = audioManager;
            View.PauseButtonClicked += OnPauseButtonClicked;
            _goalPanelController = new GoalPanelController(view.GoalPanelView);
        }

        internal void SetupUI(LevelDataSO leveldata, IReadOnlyList<LevelGoalBase> goals)
        {
            UpdateRemainingMove(leveldata.MaxMove);
            UpdateScore(0);
            UpdateProgressBar(0);
            _goalPanelController.SetupGoals(goals);
        }

        internal void ToggleHUD(bool shouldShow) => View.SetVisible(shouldShow);
        internal void UpdateRemainingMove(int remainingMove) => View.UpdateRemainingMove(remainingMove);
        internal void UpdateScore(int score) => View.UpdateScore(score);
        internal void UpdateProgressBar(float totalProgress) => View.UpdateProgressBar(totalProgress);
        internal void UpdateGoals(Dictionary<GemType, int> clearedPieces) => _goalPanelController.UpdateGoals(clearedPieces);
        private void OnPauseButtonClicked()
        {
            _audioManager.PlaySfx(SfxType.ButtonClick);
            _uiManager.ShowPopupAsync(PopupType.Pause).Forget();
        }
    }
}