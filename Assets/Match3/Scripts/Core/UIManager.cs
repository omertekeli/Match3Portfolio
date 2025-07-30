using System;
using Cysharp.Threading.Tasks;
using Match3.Scripts.Core.Events;
using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Level.Data;
using Match3.Scripts.UI;
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

        [SerializeField] private HUDView _hudView;
        [SerializeField] private FadeView _fadeView;

        private HUDController _hudController;
        private FadeController _fadeController;
        private IEventSubscriber _subscriber;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _hudController = new HUDController(_hudView);
            _fadeController = new FadeController(_fadeView);
            _subscriber = ServiceLocator.Get<IEventSubscriber>();
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            _subscriber.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        void OnDisable()
        {
            _subscriber.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        #endregion

        #region Methods

        public void RegisterLevelMenu(LevelMenu menu)
        {
            menu.LevelSelected += OnLevelSelection;
        }

        public void UnregisterLevelMenu(LevelMenu menu)
        {
            menu.LevelSelected -= OnLevelSelection;
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

        private void OnLevelSelection(int levelIndex)
        {
            ServiceLocator.Get<GameManager>().TryToLoadLevel(levelIndex);
        }

        private void OnGameStateChanged(GameStateChangedEvent eventData)
        {
            _hudController.ToggleHUD(eventData.NewState);
        }

        #endregion
    }
}