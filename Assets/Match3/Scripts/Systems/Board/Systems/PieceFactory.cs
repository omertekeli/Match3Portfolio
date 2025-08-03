using System.Collections.Generic;
using Match3.Scripts.Core;
using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Board.Contents;
using Match3.Scripts.Systems.Board.Contents.BoardPower;
using Match3.Scripts.Systems.Board.Contents.Gem;
using Match3.Scripts.Systems.Board.Contents.Obstacle;
using Match3.Scripts.Systems.Board.Contents.Obstacle.Content;
using Match3.Scripts.Systems.Board.Contents.Obstacle.Overlay;
using Match3.Scripts.Systems.Board.Data;
using UnityCoreModules.Services;
using UnityEngine;

namespace Match3.Scripts.Systems.Board.Systems
{
    public class PieceFactory: IPieceFactory
    {
        private readonly PiecePrefabDB _prefabDB;
        private readonly Transform _contentContainer;
        private readonly GemSpriteProvider _gemSpriteProvider;

        private readonly Dictionary<ObstacleType, GameObject> _obstaclePrefabDict = new();
        private readonly Dictionary<BoardPowerType, GameObject> _boardPowerPrefabDict = new();

        public PieceFactory(PiecePrefabDB prefabDB, Transform contentContainer)
        {
            _prefabDB = prefabDB;
            _contentContainer = contentContainer;
            _gemSpriteProvider = ServiceLocator.Get<GemSpriteProvider>();

            foreach (var mapping in _prefabDB.ObstaclePrefabs)
            { _obstaclePrefabDict[mapping.Data.Type] = mapping.Prefab; }
            foreach (var mapping in _prefabDB.BoardPowerPrefabs)
            { _boardPowerPrefabDict[mapping.Data.Type] = mapping.Prefab; }
        }
        public void CreateGem(TileNode node, Vector3 position, GemType gemType)
        {
            GameObject gemGO = Object.Instantiate(_prefabDB.GenericGemPrefab, position, Quaternion.identity, _contentContainer);
            gemGO.name = $"Gem_{gemType} ({node.GridPosition.x},{node.GridPosition.y})";

            var gemView = gemGO.GetComponent<GemView>();
            Sprite sprite = _gemSpriteProvider.GetSprite(gemType);
            if (gemView != null) gemView.SetSprite(sprite);

            var gem = new Gem(gemType);
            node.SetContent(gem);
        }

        public void CreateObstacle(TileNode node, Vector3 position, ObstacleDataSO data)
        {
            if (data == null || !_obstaclePrefabDict.TryGetValue(data.Type, out GameObject prefab)) return;

            Obstacle obstacleInstance = null;
            switch (data.Type)
            {
                case ObstacleType.Crate:
                    obstacleInstance = new Crate(node, data);
                    break;
                case ObstacleType.Ice:
                    obstacleInstance = new Ice(node, data);
                    break;
            }

            if (obstacleInstance == null) return;

            if (data.PlacementType == ObstaclePlacementType.Content && obstacleInstance is IBoardContent content)
            {
                node.SetContent(content);
            }
            else if (data.PlacementType == ObstaclePlacementType.Overlay && obstacleInstance is IOverlay overlay)
            {
                node.SetOverlay(overlay);
            }

            GameObject obstacleGO = Object.Instantiate(prefab, position, Quaternion.identity, _contentContainer);
            obstacleGO.name = $"Obstacle_{data.Type} ({node.GridPosition.x},{node.GridPosition.y})";
            var view = obstacleGO.GetComponent<ObstacleView>();
            view.Initialize(obstacleInstance);
        }

        public void CreateBoardPower(TileNode node, Vector3 position, BoardPowerDataSO data)
        {
            if (data == null || !_boardPowerPrefabDict.TryGetValue(data.Type, out GameObject prefab)) return;

            GameObject powerGO = Object.Instantiate(prefab, position, Quaternion.identity, _contentContainer);
            powerGO.name = $"Power_{data.Type} ({node.GridPosition.x},{node.GridPosition.y})";

            var power = new BoardPower(data);
            node.SetContent(power);
        }
    }
}