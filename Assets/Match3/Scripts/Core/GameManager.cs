using System;
using Cysharp.Threading.Tasks;
using Match3.Scripts.Configs;
using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Level.Data;
using Match3.Scripts.UI;
using UnityCoreModules.Services;
using UnityEngine;

namespace Match3.Scripts.Core
{
    public class GameManager : MonoBehaviour, IService
    {
        #region Fields

        [SerializeField] private LevelListSO _levelList;
        [SerializeField] private LevelDataSO[] _levelDatas;
        [SerializeField] private float _levelLoadingHoldTime = 1f;

        private GameState _gameCurrentState;

        #endregion

        #region Properties

        public GameState GameState { get; private set; }

        #endregion

        #region Events

        public event Action<GameState> GameStateChanged;

        #endregion

        #region Unity Methods
        private void Awake()
        {
            Application.targetFrameRate = 30;
            SetState(GameState.MainMenu);
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            LevelMenu.LevelSelected += OnLevelSelected;
            SceneLoader.LevelLoaded += OnLevelLoaded;
        }

        private void OnDisable()
        {
            LevelMenu.LevelSelected -= OnLevelSelected;
            SceneLoader.LevelLoaded -= OnLevelLoaded;
        }

        #endregion

        #region Methods

        private void OnLevelSelected(int levelNumber)
        {
            Debug.Log($"Selected level {levelNumber + 1}");
            if (!_levelList.IsLevelValid(levelNumber)) return;
            LoadLevelAsync(levelNumber).Forget();
        }

        private void OnLevelLoaded(int levelNumber)
        {
            Debug.Log($"Loaded level {levelNumber}");
            if (!_levelDatas[levelNumber]) return;
            StartLevelAsync(levelNumber).Forget();
        }

        private async UniTaskVoid StartLevelAsync(int levelNumber)
        {
            Debug.Log($"Starting level {levelNumber}");
            var levelData = _levelDatas[levelNumber];
            await ServiceLocator.Get<LevelManager>().InitializeLevelAsync(levelData);
            ServiceLocator.Get<UIManager>().SetupLevelUI(levelData);
            SetState(GameState.Gameplay);
            await ServiceLocator.Get<UIManager>().ShowLevelUIAsync(_levelLoadingHoldTime);
        }

        private async UniTaskVoid LoadLevelAsync(int levelNumber)
        {
            SetState(GameState.Loading);
            await ServiceLocator.Get<UIManager>().PlayLoadingTransitionAsync();
            ServiceLocator.Get<SceneLoader>().LoadLevel(_levelList, levelNumber);
        }

        private void SetState(GameState state)
        {
            _gameCurrentState = state;
            GameStateChanged?.Invoke(_gameCurrentState);
        }

        #endregion
    }
}