using System;
using System.Collections.Generic;
using UnityEngine;
using Match3.Scripts.Systems.Level.Data;
using Match3.Scripts.Systems.Level.Base;
using Cysharp.Threading.Tasks;

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

        public UniTask InitializeLevelAsync(LevelDataSO levelData)
        {
            RemaningMove = levelData.MaxMove;
            LevelGoals = levelData.CreateRuntimeGoals();
            return UniTask.CompletedTask;
        }
    }
}