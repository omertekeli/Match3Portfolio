using System;
using Match3.Scripts.Enums;
using Match3.Scripts.UI.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Scripts.UI.Components
{
    public class ResultPopup : UIPanel
    {
        #region Fields
        [SerializeField] private Button _replayButton;
        [SerializeField] private Button _menuButton;
        [SerializeField] private TextMeshProUGUI _scoreText;
        #endregion

        #region Properties
        public override PopupType Type => PopupType.Result;
        #endregion

        #region Actions
        public event Action<PopupActionType> ActionRequested;
        #endregion

        void Awake()
        {
            base.Awake();
            _replayButton.onClick.AddListener(() => ActionRequested?.Invoke(PopupActionType.Replay));
            _menuButton.onClick.AddListener(() => ActionRequested?.Invoke(PopupActionType.MainMenu));
        }

        public void Setup(int score)
        {
            _scoreText.text = score.ToString();
            Debug.Log("Result Popup is ready");
        }
    }
}