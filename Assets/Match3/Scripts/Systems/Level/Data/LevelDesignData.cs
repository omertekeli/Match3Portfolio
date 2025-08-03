using UnityEngine;
using System;
using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Board.Contents.BoardPower;
using Match3.Scripts.Systems.Board.Contents.Obstacle;

namespace Match3.Scripts.Systems.Level.Data
{
    public enum InitialTileType { Normal, Hole, Generator }

    public enum PredefinedContentType { RandomGem, SpecificGem, BoardPower, ContentObstacle }

    [Serializable]
    public class TileSetupData
    {
        [Tooltip("Board Tile Behaviour")]
        public InitialTileType groundType = InitialTileType.Normal;

        [Tooltip("Board Content")]
        public PredefinedContentType contentType = PredefinedContentType.RandomGem;

        //Properties will use regarding content type selection
        public GemType gemType;
        public BoardPowerDataSO powerData;
        public ObstacleDataSO contentObstacleData; //only content obstacle
        public ObstacleDataSO overlayObstacleData; //only overlay
    }
}

