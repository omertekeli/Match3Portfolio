using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Match3.Scripts.LevelSystem.Data;
using Match3.Scripts.LevelSystem.Goals;
using NUnit.Framework;
using UnityEditor;

namespace Match3.Scripts.Core
{
    public class LevelManager: MonoBehaviour, IService
    {
        #region Properties
        
        public int RemaningMove { get; private set; }
        public List<LevelGoalBase> LevelGoals {get; private set;}
        
        #endregion
        
        #region Events

        public event Action OnAllGoalCompleted;
        
        #endregion
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public Task InitializeLevelAsync(LevelDataSO levelData)
        {
            RemaningMove = levelData.MaxMove;
            LevelGoals = levelData.CreateRuntimeGoals();
            return Task.CompletedTask;
        }
    }
}