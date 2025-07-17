using System;
using System.Threading.Tasks;
using Match3.Scripts.Enums;
using Match3.Scripts.LevelSystem.Data;
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
        
        #endregion

        #region Methods
        public async Task StartLevelAsync(int levelNumber)
        {
            Debug.Log($"Starting level {levelNumber}");
            var levelData = _levelDatas[levelNumber];
            await ServiceLocator.Get<LevelManager>().InitializeLevelAsync(levelData);
            ServiceLocator.Get<UIManager>().SetupLevelUIAsync(levelData);
            SetState(GameState.Gameplay);
            await ServiceLocator.Get<UIManager>().ShowLevelUIAsync(_levelLoadingHoldTime);
        }

        public async Task LoadLevelAsync(int levelNumber)
        {
            Debug.Log($"Selected level {levelNumber + 1}");
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