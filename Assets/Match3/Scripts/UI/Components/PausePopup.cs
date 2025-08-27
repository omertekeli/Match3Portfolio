using System;
using DG.Tweening;
using Match3.Scripts.Enums;
using Match3.Scripts.UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Scripts.UI.Components
{
    public class PausePopup : UIPanel
    {
        #region Fields
        [Header("Buttons")]
        [SerializeField] private Button _musicButton;
        [SerializeField] private Button _sfxButton;
        [SerializeField] private Button _replayButton;
        [SerializeField] private Button _menuButton;
        [SerializeField] private Button _closeButton;

        [Header("Button Toggle")]
        [SerializeField] private RectTransform _musicOnPosition;
        [SerializeField] private RectTransform _musicOffPosition;
        [SerializeField] private RectTransform _sfxOnPosition;
        [SerializeField] private RectTransform _sfxOffPosition;

        private RectTransform _musicToggleHandle;
        private RectTransform _sfxToggleHandle;
        private bool _isMusicOn;
        private bool _isSfxOn;
        private float _toggleAnimDuration = 0.25f;

        #endregion

        #region Actions
        public event Action<PopupActionType> ActionRequested;
        public event Action<bool> MusicToggle;
        public event Action<bool> SfxToggle;
        #endregion

        #region Properties
        public override PopupType Type => PopupType.Pause;
        #endregion


        private void Awake()
        {
            base.Awake();
            SetHandles();
            _closeButton.onClick.AddListener(() => ActionRequested?.Invoke(PopupActionType.Resume));
            _replayButton.onClick.AddListener(() => ActionRequested?.Invoke(PopupActionType.Replay));
            _menuButton.onClick.AddListener(() => ActionRequested?.Invoke(PopupActionType.MainMenu));
            _musicButton.onClick.AddListener(ToggleMusic);
            _sfxButton.onClick.AddListener(ToggleSfx);
            Debug.Log("Pause pop up is awaken!");
        }

        public void Setup(bool isMusicOn, bool isSfxOn)
        {
            _isMusicOn = isMusicOn;
            _isSfxOn = isSfxOn;
            UpdateMusicVisuals();
            UpdateSfxVisuals();
        }

        private void ToggleMusic()
        {
            _isMusicOn = !_isMusicOn;
            UpdateMusicVisuals();
            MusicToggle?.Invoke(_isMusicOn);
        }

        private void ToggleSfx()
        {
            _isSfxOn = !_isSfxOn;
            UpdateSfxVisuals();
            SfxToggle?.Invoke(_isSfxOn);
        }

        private void UpdateMusicVisuals()
        {
            Vector2 targetPosition = _isMusicOn ? _musicOnPosition.anchoredPosition : _musicOffPosition.anchoredPosition;
            _musicToggleHandle.DOAnchorPos(targetPosition, _toggleAnimDuration).SetEase(Ease.OutCubic);
        }

        private void UpdateSfxVisuals()
        {
            Vector2 targetPosition = _isSfxOn ? _sfxOnPosition.anchoredPosition : _sfxOffPosition.anchoredPosition;
            _sfxToggleHandle.DOAnchorPos(targetPosition, _toggleAnimDuration).SetEase(Ease.OutCubic);
        }

        private void SetHandles()
        {
            if (_musicButton != null)
            {
                _musicToggleHandle = _musicButton.GetComponent<RectTransform>();
            }
            if (_sfxButton != null)
            {
                _sfxToggleHandle = _sfxButton.GetComponent<RectTransform>();
            }
        }

        private void OnDestroy()
        {
            _closeButton.onClick.RemoveAllListeners();
            _replayButton.onClick.RemoveAllListeners();
            _menuButton.onClick.RemoveAllListeners();
            _musicButton.onClick.RemoveAllListeners();
            _sfxButton.onClick.RemoveAllListeners();
        }
    }
}