using System;
using Cysharp.Threading.Tasks;
using Match3.Scripts.Core.Events;
using Match3.Scripts.Core.Interfaces;
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
        [SerializeField] private float _extraLoadingHoldDuration = 1f;

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
            _subscriber.Subscribe<LevelLoaded>(OnLevelLoaded);
        }

        void OnDisable()
        {
            _subscriber.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
            _subscriber.Unsubscribe<LevelLoaded>(OnLevelLoaded);
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

        private void OnLevelSelection(int levelIndex)
        {
            ServiceLocator.Get<GameManager>().RequestStartLevel(levelIndex);
        }

        private void OnLevelLoaded(LevelLoaded eventData)
        {
            SetupLevelUI(eventData.LevelData);
        }

        private void OnGameStateChanged(GameStateChangedEvent eventData)
        {
            _hudController.ToggleHUD(eventData.NewState);
        }

        #endregion
    }
}