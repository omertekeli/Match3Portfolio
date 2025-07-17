using Match3.Scripts.Core;
using UnityCoreModules.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Scripts.UI
{
    public class LevelMenu : MonoBehaviour
    {
        [SerializeField] private Button[] _levelButtons;

        private void Start()
        {
            for (int i = 0; i < _levelButtons.Length; i++)
            {
                Debug.Log($"Setting level {i} button call back function");
                var levelIndex = i;
               _levelButtons[i].onClick.AddListener(() =>
               {
                   Debug.Log($"Level {levelIndex} button clicked");
                   _ = ServiceLocator.Get<GameManager>().LoadLevelAsync(levelIndex);
               });
            }
        }
    }
}