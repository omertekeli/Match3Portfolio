using UnityEditor;
using UnityEngine;

namespace Match3.Scripts.LevelSystem.Data
{
    [CreateAssetMenu(menuName = "Match3/Level List")]
    public class LevelListSO: ScriptableObject
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
    }
}