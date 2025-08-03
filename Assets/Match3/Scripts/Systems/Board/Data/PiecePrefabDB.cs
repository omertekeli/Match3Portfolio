using System.Collections.Generic;
using Match3.Scripts.Systems.Board.Contents.BoardPower;
using Match3.Scripts.Systems.Board.Contents.Obstacle;
using UnityEngine;

namespace Match3.Scripts.Systems.Board.Data
{
    [System.Serializable]
    public class ObstaclePrefabMapping
    {
        public ObstacleDataSO Data;
        public GameObject Prefab;
    }

    [System.Serializable]
    public class BoardPowerPrefabMapping
    {
        public BoardPowerDataSO Data;
        public GameObject Prefab;
    }

    [CreateAssetMenu(fileName = "PiecePrefabDB", menuName = "Match3/Piece Prefab Database")]
    public class PiecePrefabDB : ScriptableObject
    {
        public GameObject GenericGemPrefab;

        [Header("Board Powers")]
        public List<BoardPowerPrefabMapping> BoardPowerPrefabs;

        [Header("Obstacles")]
        public List<ObstaclePrefabMapping> ObstaclePrefabs;
    }
}