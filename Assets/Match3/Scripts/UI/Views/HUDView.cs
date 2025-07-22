using Match3.Scripts.UI.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Scripts.UI.Views
{
    public class HUDView : UIView
    {
        #region Fields

        [SerializeField] private TextMeshProUGUI _remainingMovesText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private GoalPanelView _goalPanelView;

        #endregion

        #region Properties

        public GoalPanelView GoalPanelView => _goalPanelView;

        #endregion

        public override void SetVisible(bool shouldVisible) => base.SetVisible(shouldVisible);
        internal void UpdateRemainingMove(int leftMoves) => _remainingMovesText.text = leftMoves.ToString();
        internal void UpdateScore(int score) => _scoreText.text = score.ToString();

    }
}