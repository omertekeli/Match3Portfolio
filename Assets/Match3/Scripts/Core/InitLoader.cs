using UnityCoreModules.Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3.Scripts.Core
{
    [DefaultExecutionOrder(-9999)]
    public class InitLoader: MonoBehaviour
    {
        private void Awake()
        {
            TryToRegisterGameManager();
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }

        private static void TryToRegisterGameManager()
        {
            var gameManager = FindAnyObjectByType<GameManager>();
            if (gameManager != null)
            {
                ServiceLocator.Register(gameManager, replaceIfExists: false);
            }
            else
            {
                Debug.LogError("GameManager not found in Init scene!");
            }
        }
    } 
}

