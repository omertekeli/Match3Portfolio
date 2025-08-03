using Cysharp.Threading.Tasks;
using UnityEngine;
using Match3.Scripts.Systems.Board;
using Match3.Scripts.Systems.Level.Data;
using UnityCoreModules.Services.EventBus;
using Match3.Scripts.Systems.Level.Base;
using System.Collections.Generic;
using Match3.Scripts.Core;
using Match3.Scripts.Core.Events;
using Match3.Scripts.Core.Interfaces;

public class LevelManager : IService, ILevelManager
{
    #region Fields
    private readonly GameObject _boardPrefab;
    private readonly LevelListSO _levelList;
    private readonly ISceneLoader _sceneLoader;
    private readonly IEventPublisher _publisher;
    private Board _currentBoard;
    private List<LevelGoalBase> _levelGoals;
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
    }
    public async UniTask LoadAndSetupLevelAsync(int levelIndex)
    {
        Debug.Log($"Tryin to load level index {levelIndex}");
        if (!_levelList.IsLevelValid(levelIndex))
        {
            throw new System.ArgumentOutOfRangeException(
                nameof(levelIndex), $"Invalid level index requested: {levelIndex}"
                );
        }

        int buildIndex = _levelList.SceneBuildIndexes[levelIndex];
        await _sceneLoader.LoadSceneByIndexAsync(buildIndex);

        LevelDataSO levelData = _levelList.LevelDatas[levelIndex];
        await SpawnBoardAsync(levelData);
        
        InitializeLevelRules(levelData);

        _publisher.Fire(new LevelLoaded(levelIndex, levelData));
    }

    private async UniTask SpawnBoardAsync(LevelDataSO levelData)
    {
        if (_currentBoard != null)
            Object.Destroy(_currentBoard.gameObject);
        GameObject boardGO = Object.Instantiate(_boardPrefab);
        _currentBoard = boardGO.GetComponent<Board>();
        await _currentBoard.CreateBoardAsycnc(levelData);
    }

    private void InitializeLevelRules(LevelDataSO levelData)
    {
        RemainingMove = levelData.MaxMove;
        _levelGoals = levelData.CreateRuntimeGoals();
        Debug.Log($"Level {levelData.LevelNumber} rules initialized. Max moves: {levelData.MaxMove}");
    }
}