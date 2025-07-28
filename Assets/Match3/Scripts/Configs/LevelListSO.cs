using UnityEditor;
using UnityEngine;

namespace Match3.Scripts.Configs
{
    [CreateAssetMenu(menuName = "Match3/Level List")]
    public class LevelListSO : ScriptableObject
    {
#if UNITY_EDITOR
        public SceneAsset[] Scenes;
#endif
        [HideInInspector] public int[] SceneList;

        public int SceneCount
        {
            get
            {
#if UNITY_EDITOR
                return Scenes.Length;
#else
            return sceneList.Length;
#endif
            }
        }
        
         public bool IsLevelValid(int levelIndex)
    {
#if UNITY_EDITOR
        if (Scenes == null || levelIndex < 0 || levelIndex >= Scenes.Length)
        {
            Debug.LogError("Invalid level number or Scenes not assigned in LevelListSO.");
            return false;
        }
#else
        if (SceneList == null || levelIndex < 0 || levelIndex >= SceneList.Length)
        {
            Debug.LogError("Invalid level number or SceneList not assigned in LevelListSO.");
            return false;
        }
#endif
        return true;
    }
    }
}