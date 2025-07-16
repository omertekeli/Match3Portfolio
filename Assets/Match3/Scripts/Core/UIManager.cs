using System.Threading.Tasks;
using Match3.Scripts.LevelSystem.Data;
using Match3.Scripts.UI;
using UnityEngine;

namespace Match3.Scripts.Core
{
    public class UIManager: MonoBehaviour, IService
    {
        [SerializeField] private FadeController _fadeController;
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void PrepareLevel(LevelDataSO levelData)
        {
            //throw new System.NotImplementedException();
        }

        public async Task FadeInAsync()
        {
            await _fadeController.FadeInAsync();
        }
    }
}