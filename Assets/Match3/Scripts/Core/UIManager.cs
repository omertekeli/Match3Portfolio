using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Match3.Scripts.Core.Events;
using Match3.Scripts.Core.Interfaces;
using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Board.Events;
using Match3.Scripts.Systems.Level.Data;
using Match3.Scripts.UI.Base;
using Match3.Scripts.UI.Components;
using Match3.Scripts.UI.Controllers;
using Match3.Scripts.UI.Views;
using UnityCoreModules.Services;
using UnityCoreModules.Services.EventBus;
using UnityEngine;

namespace Match3.Scripts.Core
{
    public class UIManager : MonoBehaviour, IService
    {
        #region Fields
        [Header("Canvas")]
        [SerializeField] private Transform _hudCanvas;

        [Header("Views")]
        [SerializeField] private HUDView _hudView;
        [SerializeField] private FadeView _fadeView;

        [Header("Popups")]
        [SerializeField] private List<UIPanel> _popupPrefabs;
        private Dictionary<PopupType, UIPanel> _popupPrefabDict;
        private UIPanel _currentPopup;

        [SerializeField] private float _extraLoadingHoldDuration = 1f;

        private HUDController _hudController;
        private FadeController _fadeController;
        private IEventSubscriber _subscriber;
        private IAudioManager _audioManager;
        #endregion


        #region Methods
        private void Awake()
        {
            SetPopupDictionary();
            SetServices();
            CreateControllers();
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            _subscriber.Subscribe<LevelLoaded>(OnLevelLoaded);
            _subscriber.Subscribe<MoveCountUpdated>(OnMoveCountUpdated);
            _subscriber.Subscribe<PiecesCleared>(OnPiecesCleared);
            _subscriber.Subscribe<GoalUpdated>(OnGoalProgressUpdated);
            _subscriber.Subscribe<ScoreUpdated>(OnScoreUpdated);
        }

        void OnDisable()
        {
            _subscriber.Unsubscribe<LevelLoaded>(OnLevelLoaded);
            _subscriber.Unsubscribe<MoveCountUpdated>(OnMoveCountUpdated);
            _subscriber.Unsubscribe<PiecesCleared>(OnPiecesCleared);
            _subscriber.Unsubscribe<GoalUpdated>(OnGoalProgressUpdated);
            _subscriber.Unsubscribe<ScoreUpdated>(OnScoreUpdated);
        }

        public void RegisterLevelMenu(LevelMenu menu) => menu.LevelSelected += OnLevelSelection;

        public void UnregisterLevelMenu(LevelMenu menu) => menu.LevelSelected -= OnLevelSelection;

        public void SetupLevelUI(LevelDataSO levelData)
        {
            _hudController.SetupUI(levelData, ServiceLocator.Get<ILevelManager>().LevelGoals);
        }

        public async UniTask PlayLoadingTransitionAsync(bool isShowingLoadingScreen)
        {
            if (isShowingLoadingScreen)
            {
                await _fadeController.FadeToBlackAsync();
            }
            else
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_extraLoadingHoldDuration));
                await _fadeController.FadeToWhiteAsync();
            }
        }

        public async UniTaskVoid ShowPopupAsync(PopupType type)
        {
            if (_currentPopup != null)
                return;

            if (_popupPrefabDict.TryGetValue(type, out UIPanel prefab))
            {
                _currentPopup = Instantiate(prefab, _hudCanvas);
                switch (type)
                {
                    case PopupType.Pause:
                        SetupPausePopup();
                        break;
                    case PopupType.Result:
                        SetupResultPopup();
                        break;
                }
                await _currentPopup.ShowAsync();
            }
        }

        public async UniTask HideCurrentPopupAsync()
        {
            if (_currentPopup == null)
                return;

            UIPanel popupToHide = _currentPopup;
            _currentPopup = null;

            if (popupToHide is PausePopup pausePopup)
            {
                pausePopup.ActionRequested -= HandlePopupAction;
                pausePopup.MusicToggle -= OnMusicToggle;
                pausePopup.SfxToggle -= OnSfxToggle;
            }
            else if (popupToHide is ResultPopup resultPopup)
            {
                resultPopup.ActionRequested -= HandlePopupAction;
            }

            await popupToHide.HideAsync();
            Destroy(popupToHide.gameObject);
        }

        private void CreateControllers()
        {
            _hudController = new HUDController(_hudView, this, _audioManager);
            _fadeController = new FadeController(_fadeView);
        }

        private void SetServices()
        {
            _audioManager = ServiceLocator.Get<IAudioManager>();
            _subscriber = ServiceLocator.Get<IEventSubscriber>();
        }

        private void SetPopupDictionary()
        {
            _popupPrefabDict = new Dictionary<PopupType, UIPanel>();
            foreach (var prefab in _popupPrefabs)
            {
                _popupPrefabDict[prefab.Type] = prefab;
            }
        }

        private void SetupResultPopup()
        {
            if (_currentPopup is not ResultPopup resultPopup)
                return;

            resultPopup.Setup(ServiceLocator.Get<IScoreSystem>().CurrentScore);
            resultPopup.ActionRequested += HandlePopupAction;
        }

        private void SetupPausePopup()
        {
            if (_currentPopup is not PausePopup pausePopup)
                return;

            pausePopup.Setup(_audioManager.IsMusicEnabled, _audioManager.IsSfxEnabled);
            pausePopup.ActionRequested += HandlePopupAction;
            pausePopup.MusicToggle += OnMusicToggle;
            pausePopup.SfxToggle += OnSfxToggle;
        }

        private void OnMusicToggle(bool shoulEnable)
        {
            _audioManager.PlaySfx(SfxType.ButtonClick);
            _audioManager.SetMusicEnabled(shoulEnable);
        }

        private void OnSfxToggle(bool shoulEnable)
        {
            _audioManager.PlaySfx(SfxType.ButtonClick);
            _audioManager.SetSfxEnabled(shoulEnable);
        }

        private void HandlePopupAction(PopupActionType actionType)
        {
            _audioManager.PlaySfx(SfxType.ButtonClick);
            switch (actionType)
            {
                case PopupActionType.Resume:
                    HideCurrentPopupAsync().Forget();
                    break;

                case PopupActionType.Replay:
                    HideCurrentPopupAsync().Forget();
                    ServiceLocator.Get<GameManager>().RequestRestartLevel();
                    break;

                case PopupActionType.MainMenu:
                    HideCurrentPopupAsync().Forget();
                    ServiceLocator.Get<GameManager>().RequestReturnMainMenu();
                    _hudController.ToggleHUD(false);
                    break;

                case PopupActionType.Quit:
                    Application.Quit();
                    break;
            }
        }

        private void OnPiecesCleared(PiecesCleared eventData) => _hudController.UpdateGoals(eventData.ClearedPieces);

        private void OnLevelSelection(int levelIndex) => ServiceLocator.Get<GameManager>().RequestStartLevel(levelIndex);

        private void OnLevelLoaded(LevelLoaded eventData)
        {
            SetupLevelUI(eventData.LevelData);
            _hudController.ToggleHUD(true);
        }

        private void OnMoveCountUpdated(MoveCountUpdated eventData)
        {
            Debug.Log($"Update Remaning Move: {eventData.RemaningMove}");
            _hudController.UpdateRemainingMove(eventData.RemaningMove);
        }

        private void OnGoalProgressUpdated(GoalUpdated eventData) => _hudController.UpdateProgressBar(eventData.TotalProgress);

        private void OnScoreUpdated(ScoreUpdated eventData)
        {
            Debug.Log($"Update Score: {eventData.CurrentScore}");
            _hudController.UpdateScore(eventData.CurrentScore);
        }
        #endregion
    }
}