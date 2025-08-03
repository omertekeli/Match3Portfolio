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
    // A private field to store the index of the currently selected tile in the grid.
    private int _selectedIndex = -1;

    // --- Sprite Cache for Performance ---
    private static GemSpriteLibrarySO _gemSpriteLibrary;
    private static Dictionary<GemType, Sprite> _gemSpriteCache;

    /// <summary>
    /// Finds the necessary ScriptableObject assets from the project and caches their data.
    /// </summary>
    private void FindAndCacheAssets()
    {
        if (_gemSpriteLibrary != null) return;
        string[] guids = AssetDatabase.FindAssets("t:GemSpriteLibrarySO");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            _gemSpriteLibrary = AssetDatabase.LoadAssetAtPath<GemSpriteLibrarySO>(path);
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
        FindAndCacheAssets();
        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject, "_tileSetups");
        
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();

        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Board Tasarım Editörü (Hücre seçmek için tıklayın)", EditorStyles.boldLabel);

        SerializedProperty widthProp = serializedObject.FindProperty("_width");
        SerializedProperty heightProp = serializedObject.FindProperty("_height");
        SerializedProperty setupsProp = serializedObject.FindProperty("_tileSetups");
        
        if (setupsProp.arraySize != widthProp.intValue * heightProp.intValue)
        {
            EditorGUILayout.HelpBox("Layout is being redefined...", MessageType.Info);
        }
        else
        {
            // A grid of selectable buttons for a quick overview ---
            EditorGUILayout.BeginVertical(GUI.skin.box);
            for (int y = heightProp.intValue - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < widthProp.intValue; x++)
                {
                    int index = y * widthProp.intValue + x;
                    SerializedProperty tileSetupProp = setupsProp.GetArrayElementAtIndex(index);
                    
                    Color originalColor = GUI.backgroundColor;
                    
                    InitialTileType groundType = (InitialTileType)tileSetupProp.FindPropertyRelative("groundType").enumValueIndex;
                    if (groundType == InitialTileType.Hole) GUI.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
                    else if (groundType == InitialTileType.Generator) GUI.backgroundColor = new Color(0.7f, 1f, 0.7f, 1f);
                    
                    if (index == _selectedIndex)
                    {
                        GUI.backgroundColor = Color.Lerp(GUI.backgroundColor, Color.yellow, 0.6f);
                    }

                    if (GUILayout.Button("", GUILayout.Width(50), GUILayout.Height(50)))
                    {
                        _selectedIndex = index;
                    }
                    
                    Rect buttonRect = GUILayoutUtility.GetLastRect();
                    DrawSpritePreview(buttonRect, tileSetupProp);

                    GUI.backgroundColor = originalColor;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(20);

            // --- DETAIL VIEW: Shows the editable properties for the selected tile ---
            if (_selectedIndex != -1 && _selectedIndex < setupsProp.arraySize)
            {
                int selectedX = _selectedIndex % widthProp.intValue;
                int selectedY = _selectedIndex / widthProp.intValue;
                
                EditorGUILayout.LabelField($"Selected Cell Settings ({selectedX}, {selectedY})", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(GUI.skin.box);
                
                SerializedProperty selectedTileProp = setupsProp.GetArrayElementAtIndex(_selectedIndex);
                DrawConditionalContentFields(selectedTileProp);

                EditorGUILayout.EndVertical();
            }
        }
        
        serializedObject.ApplyModifiedProperties();
    }
    
    /// <summary>
    /// Helper method to draw the detailed property fields for a single selected tile.
    /// </summary>
    private void DrawConditionalContentFields(SerializedProperty tileSetupProp)
    {
        SerializedProperty availableTypesProp = serializedObject.FindProperty("_availablePieceTypes");
        
        EditorGUILayout.PropertyField(tileSetupProp.FindPropertyRelative("groundType"));
        
        SerializedProperty contentProp = tileSetupProp.FindPropertyRelative("contentType");
        SerializedProperty overlayProp = tileSetupProp.FindPropertyRelative("overlayObstacleData");
        
        InitialTileType groundType = (InitialTileType)tileSetupProp.FindPropertyRelative("groundType").enumValueIndex;
        bool isHole = groundType == InitialTileType.Hole;

        EditorGUI.BeginDisabledGroup(isHole);
        
        if (isHole)
        {
            contentProp.enumValueIndex = (int)PredefinedContentType.RandomGem;
            overlayProp.objectReferenceValue = null;
        }

        EditorGUILayout.PropertyField(contentProp);

        PredefinedContentType contentType = (PredefinedContentType)contentProp.enumValueIndex;
        
        switch (contentType)
        {
            case PredefinedContentType.SpecificGem:
                SerializedProperty gemProp = tileSetupProp.FindPropertyRelative("gemType");
                List<string> availableGemNames = new List<string>();
                List<int> availableGemValues = new List<int>();
                for (int i = 0; i < availableTypesProp.arraySize; i++)
                {
                    int enumIndex = availableTypesProp.GetArrayElementAtIndex(i).enumValueIndex;
                    availableGemNames.Add(((GemType)enumIndex).ToString());
                    availableGemValues.Add(enumIndex);
                }
                if (availableGemNames.Count == 0) EditorGUILayout.HelpBox("Add types to 'Available Piece Types'!", MessageType.Warning);
                else gemProp.enumValueIndex = EditorGUILayout.IntPopup("Gem Type", gemProp.enumValueIndex, availableGemNames.ToArray(), availableGemValues.ToArray());
                break;
            case PredefinedContentType.BoardPower:
                EditorGUILayout.PropertyField(tileSetupProp.FindPropertyRelative("powerData"));
                break;
            case PredefinedContentType.ContentObstacle:
                EditorGUILayout.PropertyField(tileSetupProp.FindPropertyRelative("contentObstacleData"));
                break;
        }

        bool canHaveOverlay = contentType == PredefinedContentType.RandomGem ||
                              contentType == PredefinedContentType.SpecificGem ||
                              contentType == PredefinedContentType.BoardPower;

        if (canHaveOverlay)
        {
            EditorGUILayout.PropertyField(overlayProp);
            if (overlayProp.objectReferenceValue != null)
            {
                var overlayData = overlayProp.objectReferenceValue as ObstacleDataSO;
                if (overlayData != null && overlayData.PlacementType != ObstaclePlacementType.Overlay)
                {
                    EditorGUILayout.HelpBox("Only 'Overlay' type obstacles allowed.", MessageType.Error);
                    overlayProp.objectReferenceValue = null;
                }
            }
        }
        else
        {
            overlayProp.objectReferenceValue = null;
        }
        
        EditorGUI.EndDisabledGroup();
    }

    /// <summary>
    /// Helper method to find the correct sprite and draw it in a given Rect.
    /// </summary>
    private void DrawSpritePreview(Rect position, SerializedProperty tileSetupProp)
    {
        Sprite spriteToShow = null;
        
        // Prioritize showing the overlay sprite.
        var overlayProp = tileSetupProp.FindPropertyRelative("overlayObstacleData");
        if (overlayProp.objectReferenceValue != null)
        {
            spriteToShow = (overlayProp.objectReferenceValue as ObstacleDataSO)?.Sprite;
        }
        
        // If no overlay, show the content sprite.
        if (spriteToShow == null)
        {
            var contentType = (PredefinedContentType)tileSetupProp.FindPropertyRelative("contentType").enumValueIndex;
            switch (contentType)
            {
                case PredefinedContentType.SpecificGem:
                    if (_gemSpriteCache != null)
                    {
                        var gemProp = tileSetupProp.FindPropertyRelative("gemType");
                        _gemSpriteCache.TryGetValue((GemType)gemProp.enumValueIndex, out spriteToShow);
                    }
                    break;
                case PredefinedContentType.BoardPower:
                    var powerProp = tileSetupProp.FindPropertyRelative("powerData");
                    if (powerProp.objectReferenceValue != null) spriteToShow = (powerProp.objectReferenceValue as BoardPowerDataSO)?.Sprite;
                    break;
                case PredefinedContentType.ContentObstacle:
                    var obstacleProp = tileSetupProp.FindPropertyRelative("contentObstacleData");
                    if (obstacleProp.objectReferenceValue != null) spriteToShow = (obstacleProp.objectReferenceValue as ObstacleDataSO)?.Sprite;
                    break;
            }
        }

        if (spriteToShow != null)
        {
            // Center the sprite inside the button rect.
            float smallerSize = Mathf.Min(position.width, position.height) * 0.8f;
            float xOffset = (position.width - smallerSize) / 2f;
            float yOffset = (position.height - smallerSize) / 2f;
            Rect spriteRect = new Rect(position.x + xOffset, position.y + yOffset, smallerSize, smallerSize);
            
            DrawSprite(spriteRect, spriteToShow);
        }
    }
    
    /// <summary>
    /// Low-level method to draw a sprite in the editor GUI, respecting its atlas coordinates.
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