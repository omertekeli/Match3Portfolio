using Cysharp.Threading.Tasks;
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
        private IEventPublisher _publisher;
        private UIManager _uiManager;
        private ILevelManager _levelManager;
        private ISceneLoader _sceneLoader;
        #endregion

        #region Properties
        public GameState GameState => _currentState;
        #endregion

        private void Awake()
        {
            _currentState = GameState.Loading;
            _publisher = ServiceLocator.Get<IEventPublisher>();
            _uiManager = ServiceLocator.Get<UIManager>();
            _levelManager = ServiceLocator.Get<ILevelManager>();
            _sceneLoader = ServiceLocator.Get<ISceneLoader>();
            RequestMainMenu().Forget();
        }

        public void RequestStartLevel(int levelIndex)
        {
            LoadLevelWorkflowAsync(levelIndex).Forget();
        }

        public async UniTaskVoid RequestMainMenu()
        {
            if (_currentState == GameState.MainMenu)
                return;
            await _sceneLoader.LoadSceneByIndexAsync((int)SceneIndex.MainMenu);
            SetState(GameState.MainMenu);
        }

        private async UniTaskVoid LoadLevelWorkflowAsync(int levelIndex)
        {
            if (_currentState == GameState.Loading)
                return;

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
                if (_currentState != GameState.MainMenu)
                {
                    RequestMainMenu().Forget();
                }
            }
        }

        private void SetState(GameState newState)
        {
            if (_currentState == newState)
                return;
            Debug.Log($"State changing to {newState}");
            _currentState = newState;
            _publisher.Fire(new GameStateChangedEvent(newState));
        }
    }
}