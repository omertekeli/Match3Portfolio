using DG.Tweening;
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
        [SerializeField] private Slider _progressBarSlider;
        [SerializeField] private float _barAnimationDuration = 0.3f;
        #endregion

        #region Properties
        public GoalPanelView GoalPanelView => _goalPanelView;
        #endregion

        public override void SetVisible(bool shouldVisible) => base.SetVisible(shouldVisible);
        internal void UpdateRemainingMove(int leftMoves) => _remainingMovesText.text = leftMoves.ToString();
        internal void UpdateScore(int score) => _scoreText.text = score.ToString();
        internal void UpdateProgressBar(float totalProgress)
        {
            _progressBarSlider.DOValue(totalProgress, _barAnimationDuration).SetEase(Ease.OutQuad);
        }

    }
}