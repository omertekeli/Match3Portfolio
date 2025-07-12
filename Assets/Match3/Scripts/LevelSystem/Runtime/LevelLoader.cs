using Match3.Scripts.LevelSystem.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Match3.Scripts.LevelSystem.Runtime
{
    public static class LevelLoader
    {
        public static void Load(LevelListSO levelList, int levelNumber)
        {
#if UNITY_EDITOR
            if (levelList.Scenes == null || levelNumber >= levelList.Scenes.Length)
            {
                Debug.LogError("Invalid level number or Scenes not assigned in LevelList.");
                return;
            }
            var scenePath = AssetDatabase.GetAssetPath(levelList.Scenes[levelNumber]);
            if (string.IsNullOrEmpty(scenePath))
            {
                Debug.LogError("Scene path not found for selected level.");
                return;
            }
            EditorSceneManager.LoadSceneInPlayMode(scenePath, new LoadSceneParameters(LoadSceneMode.Single));
#else            
            if (levelList.SceneList == null || levelNumber >= levelList.SceneList.Length)
            {
                Debug.LogError("Invalid level number or SceneList not assigned.");
                return;
            }
            SceneManager.LoadScene(levelList.SceneList[levelNumber], LoadSceneMode.Single);
#endif
        }
    }
}