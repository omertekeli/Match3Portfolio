using Cysharp.Threading.Tasks;
using Match3.Scripts.Configs;
using Match3.Scripts.Core.Events;
using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Level.Data;
using UnityCoreModules.Services;
using UnityCoreModules.Services.EventBus;
using UnityEngine;

namespace Match3.Scripts.Core
{
    public class GameManager : MonoBehaviour, IService
    {
        #region Fields

        [SerializeField] private LevelListSO _levelList;
        [SerializeField] private LevelDataSO[] _levelDatas;
        [SerializeField] private float _levelLoadingHoldTime = 1f;
        private GameState _currentState;
        private IEventBus _eventBus;
        private UIManager _uiManager;
        private LevelManager _levelManager;
        private SceneLoader _sceneLoader;

        #endregion

        #region Unity Methods
        private void Awake()
        {
            Application.targetFrameRate = 30;
            SetState(GameState.MainMenu);
            DontDestroyOnLoad(gameObject);
            _eventBus = ServiceLocator.Get<IEventBus>();
            _uiManager = ServiceLocator.Get<UIManager>();
            _levelManager = ServiceLocator.Get<LevelManager>();
            _sceneLoader = ServiceLocator.Get<SceneLoader>();
        }

        private void OnEnable()
        {
            _eventBus.Subscribe<LevelLoaded>(OnLevelLoaded);
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<LevelLoaded>(OnLevelLoaded);
        }

        #endregion

        #region Methods

        public void TryToLoadLevel(int levelNumber)
        {
            Debug.Log($"Selected level {levelNumber + 1}");
            if (!_levelList.IsLevelValid(levelNumber)) return;
            LoadLevelAsync(levelNumber).Forget();
        }

        private void OnLevelLoaded(LevelLoaded eventData)
        {
            Debug.Log($"Loaded level {eventData.LevelIndex}");
            if (!_levelDatas[eventData.LevelIndex]) return;
            StartLevelAsync(eventData.LevelIndex).Forget();
        }

        private async UniTaskVoid StartLevelAsync(int LevelIndex)
        {
            Debug.Log($"Starting level {LevelIndex}");
            var levelData = _levelDatas[LevelIndex];
            await _levelManager.InitializeLevelAsync(levelData);
            _uiManager.SetupLevelUI(levelData);
            SetState(GameState.Gameplay);
            await _uiManager.ShowLevelUIAsync(_levelLoadingHoldTime);
        }

        private async UniTaskVoid LoadLevelAsync(int levelNumber)
        {
            SetState(GameState.Loading);
            await _uiManager.PlayLoadingTransitionAsync();
            _sceneLoader.LoadLevel(_levelList, levelNumber);
        }

        private void SetState(GameState newState)
        {
            if (_currentState == newState) return;
            _currentState = newState;
            _eventBus.Fire(new GameStateChangedEvent(newState));
        }

        #endregion
    }
}