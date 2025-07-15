using System;
using System.Collections.Generic;
using UnityEngine;
using Match3.Scripts.LevelSystem.Data;
using Match3.Scripts.LevelSystem.Goals;

namespace Match3.Scripts.Core
{
    public class LevelManager: MonoBehaviour, IService
    {
        #region Fields

        private List<LevelGoalBase> _levelGoals;

        #endregion
        
        #region Properties
        
        public int RemaningMove { get; private set; }
        
        #endregion
        
        #region Events

        public event Action OnAllGoalCompleted;
        
        #endregion
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void PrepareLevel(LevelDataSO levelData)
        {
            RemaningMove = levelData.MaxMove;
            _levelGoals = levelData.CreateRuntimeGoals();
        }
    }
}