using System.Threading.Tasks;
using Match3.Scripts.Enums;
using Match3.Scripts.LevelSystem.Data;
using UnityCoreModules.Services;
using UnityEngine;

namespace Match3.Scripts.Core
{
    public class GameManager: MonoBehaviour, IService
    {
        #region Fields

        [SerializeField] private LevelListSO _levelList;
        [SerializeField] private LevelDataSO[] _levelDatas;

        private GameState _gameCurrentState;
    
        #endregion
        
        #region Properties
        
        public GameState GameState { get; private set; }
        
        #endregion
        private void Awake()
        {
            Application.targetFrameRate = 30;
            SetState(GameState.MainMenu);
            DontDestroyOnLoad(gameObject);
        }
        
        public async Task StartLevelAsync(int levelNumber)
        {
            Debug.Log($"Starting level {levelNumber}");
            SetState(GameState.Gameplay);
            var levelData = _levelDatas[levelNumber];
            ServiceLocator.Get<LevelManager>().PrepareLevel(levelData);
            ServiceLocator.Get<UIManager>().PrepareLevel(levelData);
        }
        
        public async Task LoadLevelAsync(int levelNumber)
        {   
            Debug.Log($"Selected level {levelNumber + 1}");
            SetState(GameState.Loading);
            await ServiceLocator.Get<UIManager>().FadeInAsync();
            ServiceLocator.Get<SceneLoader>().LoadLevel(_levelList, levelNumber);
        }

        private void SetState(GameState state)
        {
            _gameCurrentState = state;
        }
    }
}