using UnityEditor;
using UnityEngine;
using Match3.Scripts.Systems.Level.Data;
using System.Collections.Generic;
using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Board.Contents.BoardPower;
using Match3.Scripts.Systems.Board.Contents.Obstacle;
using Match3.Scripts.Configs;

[CustomEditor(typeof(LevelDataSO))]
public class LevelDataSOEditor : Editor
{
    // --- Sprite Cache for Performance ---
    // We store references here to avoid searching the project for assets repeatedly.
    private static GemSpriteLibrarySO _gemSpriteLibrary;
    private static Dictionary<GemType, Sprite> _gemSpriteCache;

    /// <summary>
    /// Finds the necessary ScriptableObject assets from the project and caches their data.
    /// </summary>
    private void FindAndCacheAssets()
    {
        // We only need to do this once per editor session.
        if (_gemSpriteLibrary != null && _gemSpriteCache != null) return;
        
        // Find all assets of type GemSpriteLibrarySO in the project.
        string[] guids = AssetDatabase.FindAssets("t:GemSpriteLibrarySO");
        if (guids.Length > 0)
        {
            // Get the path of the first found asset.
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            _gemSpriteLibrary = AssetDatabase.LoadAssetAtPath<GemSpriteLibrarySO>(path);

            // Populate the dictionary for fast sprite lookups.
            _gemSpriteCache = new Dictionary<GemType, Sprite>();
            if (_gemSpriteLibrary != null)
            {
                foreach (var pair in _gemSpriteLibrary.GemSprites)
                {
                    _gemSpriteCache[pair.Type] = pair.Sprite;
                }
            }
        }
    }

    /// <summary>
    /// The main method that draws the entire custom inspector.
    /// </summary>
    public override void OnInspectorGUI()
    {
        // Make sure our sprite cache is populated.
        FindAndCacheAssets();
        
        // These two commands are essential for making sure the editor handles data correctly (Undo, Saving, etc.).
        serializedObject.Update();

        // Draw all default properties EXCEPT the one we are going to draw manually.
        DrawPropertiesExcluding(serializedObject, "_tileSetups");
        
        // Apply changes to properties like Width/Height immediately to help OnValidate fire faster.
        serializedObject.ApplyModifiedProperties(); 
        serializedObject.Update();

        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Board Tasarım Editörü", EditorStyles.boldLabel);

        // Get the SerializedProperty objects we need to work with.
        SerializedProperty widthProp = serializedObject.FindProperty("_width");
        SerializedProperty heightProp = serializedObject.FindProperty("_height");
        SerializedProperty setupsProp = serializedObject.FindProperty("_tileSetups");
        SerializedProperty availableTypesProp = serializedObject.FindProperty("_availablePieceTypes");
        
        // Safety check to prevent errors when resizing the board.
        if (setupsProp.arraySize != widthProp.intValue * heightProp.intValue)
        {
            EditorGUILayout.HelpBox("Genişlik veya Yükseklik değiştirildi, dizi yeniden boyutlandırılıyor...", MessageType.Info);
        }
        else
        {
            // If dimensions match the array size, draw the grid safely.
            EditorGUILayout.BeginVertical(GUI.skin.box);
            for (int y = 0; y < heightProp.intValue; y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < widthProp.intValue; x++)
                {
                    int index = y * widthProp.intValue + x;
                    SerializedProperty tileSetupProp = setupsProp.GetArrayElementAtIndex(index);
                    
                    Color originalColor = GUI.backgroundColor;
                    
                    // --- Determine Cell Color ---
                    InitialTileType groundType = (InitialTileType)tileSetupProp.FindPropertyRelative("groundType").enumValueIndex;
                    if (groundType == InitialTileType.Hole) GUI.backgroundColor = Color.black;
                    else if (groundType == InitialTileType.Generator) GUI.backgroundColor = new Color(0.7f, 1f, 0.7f);
                    
                    PredefinedContentType contentType = (PredefinedContentType)tileSetupProp.FindPropertyRelative("contentType").enumValueIndex;
                    if(contentType != PredefinedContentType.None) GUI.backgroundColor = Color.Lerp(GUI.backgroundColor, Color.cyan, 0.3f);

                    EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(120), GUILayout.Height(85));

                    // --- Draw Cell Controls ---
                    SerializedProperty groundProp = tileSetupProp.FindPropertyRelative("groundType");
                    SerializedProperty contentProp = tileSetupProp.FindPropertyRelative("contentType");

                    groundProp.enumValueIndex = (int)(InitialTileType)EditorGUILayout.EnumPopup((InitialTileType)groundProp.enumValueIndex);
                    
                    bool isHole = (InitialTileType)groundProp.enumValueIndex == InitialTileType.Hole;
                    EditorGUI.BeginDisabledGroup(isHole);
                    if (isHole) contentProp.enumValueIndex = (int)PredefinedContentType.None;
                    
                    contentProp.enumValueIndex = (int)(PredefinedContentType)EditorGUILayout.EnumPopup((PredefinedContentType)contentProp.enumValueIndex);
                    EditorGUI.EndDisabledGroup();

                    Sprite spriteToShow = null;
                    switch ((PredefinedContentType)contentProp.enumValueIndex)
                    {
                        case PredefinedContentType.SpecificGem:
                            SerializedProperty gemProp = tileSetupProp.FindPropertyRelative("gemType");

                            // --- DYNAMIC DROPDOWN LOGIC ---
                            List<string> availableGemNames = new List<string>();
                            List<int> availableGemValues = new List<int>();
                            for (int i = 0; i < availableTypesProp.arraySize; i++)
                            {
                                int enumIndex = availableTypesProp.GetArrayElementAtIndex(i).enumValueIndex;
                                availableGemNames.Add(((GemType)enumIndex).ToString());
                                availableGemValues.Add(enumIndex);
                            }

                            gemProp.enumValueIndex = EditorGUILayout.IntPopup(gemProp.enumValueIndex, availableGemNames.ToArray(), availableGemValues.ToArray());
                            // --- END DYNAMIC DROPDOWN ---
                            
                            if (_gemSpriteCache != null) _gemSpriteCache.TryGetValue((GemType)gemProp.enumValueIndex, out spriteToShow);
                            break;

                        case PredefinedContentType.BoardPower:
                            SerializedProperty powerProp = tileSetupProp.FindPropertyRelative("powerData");
                            EditorGUILayout.PropertyField(powerProp, GUIContent.none);
                            if(powerProp.objectReferenceValue != null) spriteToShow = (powerProp.objectReferenceValue as BoardPowerDataSO)?.Sprite;
                            break;

                        case PredefinedContentType.Obstacle:
                            SerializedProperty obstacleProp = tileSetupProp.FindPropertyRelative("obstacleData");
                            EditorGUILayout.PropertyField(obstacleProp, GUIContent.none);
                            if(obstacleProp.objectReferenceValue != null) spriteToShow = (obstacleProp.objectReferenceValue as ObstacleDataSO)?.Sprite;
                            break;
                    }
                    
                    if (spriteToShow != null)
                    {
                        Rect spriteRect = GUILayoutUtility.GetRect(25, 25);
                        DrawSprite(spriteRect, spriteToShow);
                    }

                    EditorGUILayout.EndVertical();
                    GUI.backgroundColor = originalColor; // Restore original color
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        
        serializedObject.ApplyModifiedProperties();
    }
    
    /// <summary>
    /// Draws a sprite in the editor GUI, respecting its atlas coordinates.
    /// </summary>
    private void DrawSprite(Rect position, Sprite sprite)
    {
        if (sprite == null) return;
        Rect textureRect = sprite.textureRect;
        Texture2D atlasTexture = sprite.texture;
        if (atlasTexture == null) return;
        Rect uvCoords = new Rect(
            textureRect.x / atlasTexture.width,
            textureRect.y / atlasTexture.height,
            textureRect.width / atlasTexture.width,
            textureRect.height / atlasTexture.height
        );
        GUI.DrawTextureWithTexCoords(position, atlasTexture, uvCoords);
    }
}