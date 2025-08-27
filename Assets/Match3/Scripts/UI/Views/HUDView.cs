using System;
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
        [Header("Displays")]
        [SerializeField] private TextMeshProUGUI _remainingMovesText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private GoalPanelView _goalPanelView;
        [SerializeField] private Slider _progressBarSlider;

        [Header("Buttons")]
        [SerializeField] private Button _pauseButton;

        [SerializeField] private float _barAnimationDuration = 0.3f;
        #endregion

        #region Properties
        public GoalPanelView GoalPanelView => _goalPanelView;
        #endregion

        #region Actions
        public event Action PauseButtonClicked;
        #endregion
        private void Awake()
        {
            _pauseButton.onClick.AddListener(() => PauseButtonClicked?.Invoke());
        }
        private void OnDestroy()
        {
            _pauseButton.onClick.RemoveAllListeners();
        }

        public override void SetVisible(bool shouldVisible) => base.SetVisible(shouldVisible);

        internal void UpdateRemainingMove(int leftMoves) => _remainingMovesText.text = leftMoves.ToString();
        internal void UpdateScore(int score) => _scoreText.text = score.ToString();
        internal void UpdateProgressBar(float totalProgress)
        {
            _progressBarSlider.DOValue(totalProgress, _barAnimationDuration).SetEase(Ease.OutQuad);
        }

    }
}