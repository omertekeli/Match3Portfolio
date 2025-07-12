using System;
using UnityEngine;
using Match3.Scripts.LevelSystem.Data;

namespace Match3.Scripts.LevelSystem.Runtime
{
    public class LevelManager: MonoBehaviour
    {
        #region Fields
    
        [SerializeField] private LevelDataSO levelData;
        
        #endregion
        
        #region Properties
        
        public int RemaningMove { get; private set; }
        
        #endregion
        
        #region Events

        public event Action OnAllGoalCompleted;
        
        #endregion

        private void Awake()
        {
            //ServiceLocator.Register(this, replaceIfExists:true);
        }

        private void OnDestroy()
        {
            //ServiceLocator.Unregister<LevelManager>();
        }
        
        private void Initialize()
        {
            RemaningMove = levelData.MaxMove;
        }
    }
}