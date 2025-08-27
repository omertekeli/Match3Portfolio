using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using Match3.Scripts.Core.Events;
using Match3.Scripts.Core.Interfaces;
using Match3.Scripts.Enums;
using UnityCoreModules.Services;
using UnityCoreModules.Services.EventBus;
using UnityEngine;

namespace Match3.Scripts.Core
{
    public class GameManager : MonoBehaviour, IService
    {
        #region Fields
        private GameState _currentState;
        private IEventBus _eventBus;
        private UIManager _uiManager;
        private ILevelManager _levelManager;
        private ISceneLoader _sceneLoader;
        #endregion

        #region Properties
        public GameState GameState => _currentState;
        #endregion

        #region Methods
        private void Awake()
        {
            _currentState = GameState.Loading;
            _eventBus = ServiceLocator.Get<IEventBus>();
            _uiManager = ServiceLocator.Get<UIManager>();
            _levelManager = ServiceLocator.Get<ILevelManager>();
            _sceneLoader = ServiceLocator.Get<ISceneLoader>();
            Init().Forget();
            DontDestroyOnLoad(gameObject);
        }

        void OnEnable()
        {
            Debug.Log("Game Manager is enabled");
            _eventBus.Subscribe<LevelEnd>(OnLevelEnd);
        }

        void OnDisable()
        {
            Debug.Log("Game Manager is disabled");
            _eventBus.Unsubscribe<LevelEnd>(OnLevelEnd);
        }

        public void RequestStartLevel(int levelIndex)
        {
            if (_currentState != GameState.MainMenu)
                return;
            if (_currentState == GameState.Loading)
                return;
            LoadLevelWorkflowAsync(levelIndex).Forget();
        }

        public void RequestRestartLevel()
        {
            if (_currentState == GameState.Loading)
                return;
            RestartLevelAsync().Forget();
        }

        public void RequestReturnMainMenu()
        {
            if (_currentState == GameState.Loading)
                return;
            ReturnMainMenuAsync().Forget();
        }

        private async UniTaskVoid Init()
        {
            await _sceneLoader.LoadSceneByIndexAsync((int)SceneIndex.MainMenu);
            _currentState = GameState.MainMenu;
        }

        private async UniTaskVoid RestartLevelAsync()
        {
            try
            {
                SetState(GameState.Loading);
                await _uiManager.PlayLoadingTransitionAsync(true);
                await _levelManager.RestartLevelAsync();
                await _uiManager.PlayLoadingTransitionAsync(false);
                await _levelManager.PlayLevelIntroAnimationAsync();
                SetState(GameState.GameOver);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to transition to state {GameState.Gameplay}. Error: {e.Message}\n{e.StackTrace}");
            }
        }

        private async UniTaskVoid ReturnMainMenuAsync()
        {
            try
            {
                SetState(GameState.Loading);
                await _uiManager.PlayLoadingTransitionAsync(true);
                await _sceneLoader.LoadSceneByIndexAsync((int)SceneIndex.MainMenu);
                await _uiManager.PlayLoadingTransitionAsync(false);
                SetState(GameState.MainMenu);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to transition to state {GameState.MainMenu}. Error: {e.Message}\n{e.StackTrace}");
            }
        }

        private async UniTaskVoid LoadLevelWorkflowAsync(int levelIndex)
        {
            try
            {
                SetState(GameState.Loading);
                await _uiManager.PlayLoadingTransitionAsync(true);
                await _levelManager.LoadAndSetupLevelAsync(levelIndex);
                await _uiManager.PlayLoadingTransitionAsync(false);
                await _levelManager.PlayLevelIntroAnimationAsync();
                SetState(GameState.Gameplay);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to transition to state {GameState.Gameplay}. Error: {e.Message}\n{e.StackTrace}");
            }
        }

        private void SetState(GameState newState)
        {
            if (_currentState == newState)
                return;
            Debug.Log($"State changing to {newState}");
            _currentState = newState;
            _eventBus.Fire(new GameStateChangedEvent(newState));
        }

        private void OnLevelEnd(LevelEnd eventData)
        {
            var state = eventData.WasWon ? GameState.Victory : GameState.GameOver;
            SetState(state);
            _uiManager.ShowPopupAsync(PopupType.Result).Forget();
        }

        #endregion
    }
}