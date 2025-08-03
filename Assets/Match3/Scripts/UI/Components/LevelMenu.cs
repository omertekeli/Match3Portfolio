using System;
using Match3.Scripts.Core;
using UnityCoreModules.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Scripts.UI
{
    public class LevelMenu : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Button[] _levelButtons;

        #endregion

        #region Events

        public event Action<int> LevelSelected;

        #endregion

        private void Start()
        {
            Setup();
            if (ServiceLocator.IsAvailable<UIManager>())
            {
                ServiceLocator.Get<UIManager>().RegisterLevelMenu(this);
            }
        }

        private void OnDestroy()
        {
            if (ServiceLocator.IsAvailable<UIManager>())
            {
                ServiceLocator.Get<UIManager>().UnregisterLevelMenu(this);
            }
        }

        private void Setup()
        {
            for (int i = 0; i < _levelButtons.Length; i++)
            {
                Debug.Log($"Setting level {i} button call back function");
                var levelIndex = i;
                _levelButtons[i].onClick.AddListener(() =>
                {
                    Debug.Log($"Level {levelIndex} button clicked");
                    LevelSelected?.Invoke(levelIndex);
                });
            }
        }
    }
}