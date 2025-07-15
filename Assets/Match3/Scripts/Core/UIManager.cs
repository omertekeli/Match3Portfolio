using Match3.Scripts.LevelSystem.Data;
using UnityEngine;

namespace Match3.Scripts.Core
{
    public class UIManager: MonoBehaviour, IService
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void PrepareLevel(LevelDataSO levelData)
        {
            //throw new System.NotImplementedException();
        }
    }
}