using System;
using System.Linq;
using Match3.Scripts.Configs;
using Match3.Scripts.Systems.Level.Data;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildLevelList : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("BuildLevelList: Pre-processing build, synchronizing LevelListSO...");

        // Find the main LevelListSO asset in the project.
        string[] guids = AssetDatabase.FindAssets("t:LevelListSO");
        if (guids.Length == 0)
        {
            throw new BuildFailedException("BuildLevelList: Could not find a LevelList asset in the project.");
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        LevelListSO levelList = AssetDatabase.LoadAssetAtPath<LevelListSO>(path);
        
        if (levelList.EditorLevels == null || levelList.EditorLevels.Length == 0)
        {
            throw new BuildFailedException("BuildLevelList: The 'EditorLevels' array in the LevelList is empty.");
        }

        var buildScenes = EditorBuildSettings.scenes.ToList();
        
        // Create lists to hold the processed data.
        var runtimeLevelDatas = new LevelDataSO[levelList.EditorLevels.Length];
        var runtimeSceneIndexes = new int[levelList.EditorLevels.Length];

        bool buildSettingsChanged = false;
        
        for (int i = 0; i < levelList.EditorLevels.Length; i++)
        {
            var mapping = levelList.EditorLevels[i];
            var sceneAsset = mapping.Scene;

            if (sceneAsset == null || mapping.Data == null)
            {
                throw new BuildFailedException($"BuildLevelList: Entry {i} in LevelListSO is invalid. Scene or Data is null.");
            }

            string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
            int sceneIndexInBuildSettings = buildScenes.FindIndex(s => s.path == scenePath);

            // If the scene is not in the build settings, add it.
            if (sceneIndexInBuildSettings == -1)
            {
                sceneIndexInBuildSettings = buildScenes.Count;
                buildScenes.Add(new EditorBuildSettingsScene(scenePath, true));
                buildSettingsChanged = true;
                Debug.Log($"BuildLevelList: Added '{scenePath}' to Build Settings at index {sceneIndexInBuildSettings}.");
            }
            // If the scene is in the build settings but disabled, enable it.
            else if (!buildScenes[sceneIndexInBuildSettings].enabled)
            {
                buildScenes[sceneIndexInBuildSettings].enabled = true;
                buildSettingsChanged = true;
                Debug.Log($"BuildLevelList: Enabled '{scenePath}' in Build Settings.");
            }
            
            // Store the final data for runtime.
            runtimeSceneIndexes[i] = sceneIndexInBuildSettings;
            runtimeLevelDatas[i] = mapping.Data;
        }

        // Update the LevelListSO asset with the new runtime data.
        levelList.SceneBuildIndexes = runtimeSceneIndexes;
        levelList.LevelDatas = runtimeLevelDatas;
        EditorUtility.SetDirty(levelList);
        AssetDatabase.SaveAssets();
        
        // If we changed the build settings, update them.
        if (buildSettingsChanged)
        {
            EditorBuildSettings.scenes = buildScenes.ToArray();
            Debug.Log("BuildLevelList: Updated EditorBuildSettings.scenes.");
        }
        
        Debug.Log("BuildLevelList: Synchronization complete.");
    }
}