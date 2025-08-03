using UnityEditor;
using UnityEngine;
using Match3.Scripts.Systems.Level.Data;

[CreateAssetMenu(menuName = "Match3/Level List")]
public class LevelListSO : ScriptableObject
{
#if UNITY_EDITOR
    public LevelMapping[] EditorLevels;
#endif

    [HideInInspector] public int[] SceneBuildIndexes;
    [HideInInspector] public LevelDataSO[] LevelDatas;

    public int SceneCount => LevelDatas?.Length ?? 0;

    public bool IsLevelValid(int levelIndex)
    {
        return LevelDatas != null && levelIndex >= 0 && levelIndex < SceneCount;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (EditorLevels == null) return;

        LevelDatas = new LevelDataSO[EditorLevels.Length];
        SceneBuildIndexes = new int[EditorLevels.Length];

        var buildScenes = EditorBuildSettings.scenes;

        for (int i = 0; i < EditorLevels.Length; i++)
        {
            var mapping = EditorLevels[i];
            LevelDatas[i] = mapping.Data;

            if (mapping.Scene == null)
            {
                SceneBuildIndexes[i] = -1;
                continue;
            }

            string scenePath = AssetDatabase.GetAssetPath(mapping.Scene);

            int sceneIndexInBuildSettings = -1;
            for (int j = 0; j < buildScenes.Length; j++)
            {
                if (buildScenes[j].path == scenePath)
                {
                    sceneIndexInBuildSettings = j;
                    break;
                }
            }

            SceneBuildIndexes[i] = sceneIndexInBuildSettings;
            if (sceneIndexInBuildSettings == -1)
            {
                Debug.LogWarning($"LevelListSO Warning: Scene '{mapping.Scene.name}' is not in the Build Settings. " +
                                 $"It will not be loaded in a build unless the BuildLevelList processor adds it.");
            }
        }

        // Değişikliklerin asset dosyasına kaydedildiğinden emin ol.
        EditorUtility.SetDirty(this);
    }
#endif
}


#if UNITY_EDITOR
[System.Serializable]
public struct LevelMapping
{
    public SceneAsset Scene;
    public LevelDataSO Data;
}
#endif