using Match3.Scripts.Enums;
using Match3.Scripts.LevelSystem.Data;
using UnityCoreModules.Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3.Scripts.Core
{
    public class SceneLoader : MonoBehaviour, IService
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void LoadSceneByIndex(int sceneIndex)
        {
            if (sceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
            }
            else
            {
                Debug.LogError($"Scene index {sceneIndex} is not in Build Settings.");
            }
        }

        public void LoadLevel(LevelListSO levelList, int levelNumber)
        {
#if UNITY_EDITOR
            if (levelList.Scenes == null || levelNumber >= levelList.Scenes.Length)
            {
                Debug.LogError("Invalid level number or Scenes not assigned in LevelList.");
                return;
            }

            var scenePath = UnityEditor.AssetDatabase.GetAssetPath(levelList.Scenes[levelNumber]);
            if (string.IsNullOrEmpty(scenePath))
            {
                Debug.LogError("Scene path not found for selected level.");
                return;
            }

            UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(
                scenePath, new LoadSceneParameters(LoadSceneMode.Single));
#else
            if (levelList.SceneList == null || levelNumber >= levelList.SceneList.Length)
            {
                Debug.LogError("Invalid level number or SceneList not assigned.");
                return;
            }

            SceneManager.LoadScene(levelList.SceneList[levelNumber], LoadSceneMode.Single);
#endif
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Notify(scene);
        }
        
        private static int ExtractLevelNumberFromSceneName(string sceneName)
        {
            const string PREFIX = "Level";
            if (!sceneName.StartsWith(PREFIX)) return -1;
            var numberStr = sceneName.Substring(PREFIX.Length);
            if (int.TryParse(numberStr, out int number))
                return number;
            return -1;
        }

        private void Notify(Scene scene)
        {
            Debug.Log($"Scene loaded: {scene.name}");
            int levelNumber = - 1; 
#if UNITY_EDITOR
            levelNumber = ExtractLevelNumberFromSceneName(scene.name);
#else
            if (scene.buildIndex <= (int)SceneIndex.MainMenu)
                return;
            levelNumber = scene.buildIndex - 1;
#endif
            if (levelNumber == -1)
            {
                Debug.LogWarning("Could not extract level number from scene name.");
                return;
            }
            ServiceLocator.Get<GameManager>().StartLevel(levelNumber);
        }
    }
}
