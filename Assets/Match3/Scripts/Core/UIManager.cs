using System.Threading.Tasks;
using Match3.Scripts.Enums;
using Match3.Scripts.LevelSystem.Data;
using Match3.Scripts.UI;
using UnityCoreModules.Services;
using UnityEngine;

namespace Match3.Scripts.Core
{
    public class UIManager : MonoBehaviour, IService
    {
        #region Fields

        [SerializeField] private FadeController _fadeController;
        [SerializeField] private HUDController _hudController;

        #endregion
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            ServiceLocator.Get<GameManager>().GameStateChanged += OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState gameState)
        {   
            _hudController.ToggleHUD(gameState);
        }

        public void SetupLevelUIAsync(LevelDataSO levelData)
        {
            _hudController.SetupUI(levelData, ServiceLocator.Get<LevelManager>().LevelGoals);
        }

        public async Task ShowLevelUIAsync(float holdTime = 1f)
        {
            await Task.Delay((int)(holdTime * 1000));
            await _fadeController.FadeToWhiteAsync(); 
            _fadeController.ToggleFadeImage(false);
        }

        public async Task PlayLoadingTransitionAsync()
        {
            _fadeController.ToggleFadeImage(true);
            await _fadeController.FadeToBlackAsync();
        }
    }
}