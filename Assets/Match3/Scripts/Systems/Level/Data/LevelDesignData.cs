using UnityEngine;
using System;
using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Board.Contents.BoardPower;
using Match3.Scripts.Systems.Board.Contents.Obstacle;

public enum InitialTileType { Normal, Hole, Generator }

// None == placeholder and spawn a gem when game started on this tile
public enum PredefinedContentType { None, SpecificGem, BoardPower, Obstacle }

[Serializable]
public class TileSetupData
{
    [Tooltip("Board Tile Behaviour")]
    public InitialTileType groundType = InitialTileType.Normal;
    
    [Tooltip("Borad Content")]
    public PredefinedContentType contentType = PredefinedContentType.None;
    
    //Properties will use regarding content type selection
    public GemType gemType;
    public BoardPowerDataSO powerData;
    public ObstacleDataSO obstacleData;
}