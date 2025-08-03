using UnityEngine;
using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Board;
using Match3.Scripts.Systems.Board.Contents.Obstacle;
using Match3.Scripts.Systems.Board.Contents.BoardPower;
using Match3.Scripts.Systems.Board.Data;

namespace Match3.Scripts.Systems.Board.Systems
{
    public interface IPieceFactory
    {
        /// <summary>
        /// Creates a Gem of a specific type on a given node.
        /// </summary>
        void CreateGem(TileNode node, Vector3 position, GemType gemType);

        /// <summary>
        /// Creates an Obstacle based on its ScriptableObject data on a given node.
        /// </summary>
        void CreateObstacle(TileNode node, Vector3 position, ObstacleDataSO data);

        /// <summary>
        /// Creates a Board Power based on its ScriptableObject data on a given node.
        /// </summary>
        void CreateBoardPower(TileNode node, Vector3 position, BoardPowerDataSO data);
    }
}