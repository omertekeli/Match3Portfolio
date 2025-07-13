using Match3.Scripts.LevelSystem.Data;
using Match3.Scripts.LevelSystem.Runtime;
using UnityEngine;

namespace Match3.Scripts.Core
{
    public class GameManager: MonoBehaviour
    {
        #region Fields

        [SerializeField] private LevelListSO _levelList;
    
        #endregion
        private void Awake()
        {
            Application.targetFrameRate = 30;
            DontDestroyOnLoad(this);
        }

        public void LoadLevel(int levelNumber)
        {   
            Debug.Log($"Selected level {levelNumber + 1}");
            LevelLoader.Load(_levelList, levelNumber);
        }
        
           
    }
}