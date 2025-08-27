using Cysharp.Threading.Tasks;
using UnityEngine;
using Match3.Scripts.Systems.Board;
using Match3.Scripts.Systems.Level.Data;
using UnityCoreModules.Services.EventBus;
using Match3.Scripts.Systems.Level.Base;
using System.Collections.Generic;
using Match3.Scripts.Core.Events;
using Match3.Scripts.Core.Interfaces;
using UnityCoreModules.Services;

namespace Match3.Scripts.Core
{
    public class LevelManager : ILevelManager
    {
        #region Fields
        private readonly GameObject _boardPrefab;
        private readonly LevelListSO _levelList;
        private readonly ISceneLoader _sceneLoader;
        private readonly IEventPublisher _publisher;
        private Board _currentBoard;
        private List<LevelGoalBase> _levelGoals;
        private int _currentLevelIndex;
        #endregion

        #region Properties
        public int RemainingMove { get; private set; }
        public IReadOnlyList<LevelGoalBase> LevelGoals => _levelGoals;
        #endregion

        public LevelManager(
            GameObject boardPrefab, LevelListSO levelList, ISceneLoader sceneLoader, IEventPublisher publisher)
        {
            _boardPrefab = boardPrefab;
            _levelList = levelList;
            _sceneLoader = sceneLoader;
            _publisher = publisher;
            _currentLevelIndex = 0;
        }

        public async UniTask LoadAndSetupLevelAsync(int levelIndex)
        {
            Debug.Log($"Trying to load level index {levelIndex}");
            if (!_levelList.IsLevelValid(levelIndex))
            {
                throw new System.ArgumentOutOfRangeException(
                    nameof(levelIndex), $"Invalid level index requested: {levelIndex}"
                    );
            }

            int buildIndex = _levelList.SceneBuildIndexes[levelIndex];
            await _sceneLoader.LoadSceneByIndexAsync(buildIndex);

            _currentLevelIndex = levelIndex;

            LevelDataSO levelData = _levelList.LevelDatas[levelIndex];
            await InitializeLevelRules(levelData);
            _publisher.Fire(new LevelLoaded(levelIndex, levelData));

            SpawnBoard(levelData);
        }

        public async UniTask PlayLevelIntroAnimationAsync()
        {
            Debug.Log("$Level Manager is starting the intro animation");
            await _currentBoard.PlayIntroAnimationAsync();
        }

        public async UniTask RestartLevelAsync()
        {
            LevelDataSO levelData = _levelList.LevelDatas[_currentLevelIndex];
            await InitializeLevelRules(levelData);
            _publisher.Fire(new LevelLoaded(_currentLevelIndex, levelData));

            SpawnBoard(levelData);
        }

        public void DecrementMove()
        {
            if (RemainingMove <= 0)
                return;

            Debug.Log($" Fire new Remaning move: {RemainingMove}");

            RemainingMove--;
            _publisher.Fire(new MoveCountUpdated(RemainingMove));

            var goalSystem = ServiceLocator.Get<GoalSystem>();
            if (RemainingMove <= 0 && !goalSystem.IsAllGoalsCompleted())
            {
                _publisher.Fire(new LevelEnd(false));
            }
        }

        private void SpawnBoard(LevelDataSO levelData)
        {
            if (_currentBoard != null)
                Object.Destroy(_currentBoard.gameObject);
            GameObject boardGO = Object.Instantiate(_boardPrefab);
            _currentBoard = boardGO.GetComponent<Board>();
            _currentBoard.CreateBoard(levelData);
        }

        private UniTask InitializeLevelRules(LevelDataSO levelData)
        {
            RemainingMove = levelData.MaxMove;
            _levelGoals = levelData.CreateRuntimeGoals();
            ServiceLocator.Get<GoalSystem>().Initialize(_levelGoals);
            ServiceLocator.Get<IScoreSystem>().Initialize();
            Debug.Log($"Level {levelData.LevelNumber} rules initialized. Max moves: {levelData.MaxMove}");
            return UniTask.CompletedTask;
        }
    }
}