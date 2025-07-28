using System;
using Cysharp.Threading.Tasks;
using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Level.Data;
using Match3.Scripts.UI.Controllers;
using Match3.Scripts.UI.Views;
using UnityCoreModules.Services;
using UnityEngine;

namespace Match3.Scripts.Core
{
    public class UIManager : MonoBehaviour, IService
    {
        #region Fields

        [SerializeField] private HUDView _hudView;
        [SerializeField] private FadeView _fadeView;
        
        private HUDController _hudController;
        private FadeController _fadeController;

        #endregion

        private void Awake()
        {
            _hudController = new HUDController(_hudView);
            _fadeController = new FadeController(_fadeView);
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

        public void SetupLevelUI(LevelDataSO levelData)
        {
            _hudController.SetupUI(levelData, ServiceLocator.Get<LevelManager>().LevelGoals);
        }

        public async UniTask ShowLevelUIAsync(float holdTime = 1f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(holdTime));
            await _fadeController.FadeToWhiteAsync();
        }

        public async UniTask PlayLoadingTransitionAsync()
        {
            await _fadeController.FadeToBlackAsync();
        }
    }
}