using UnityEngine;
using System.Collections.Generic;
using Match3.Scripts.Core;
using Match3.Scripts.Systems.Board.Data;
using Match3.Scripts.Systems.Level.Data;
using Match3.Scripts.Systems.Board.Contents.Gem;
using Match3.Scripts.Systems.Board.Contents.Obstacle;
using Match3.Scripts.Systems.Board.Contents.BoardPower;
using UnityCoreModules.Services;
using Match3.Scripts.Systems.Board.Contents.Obstacle.Content;
using Match3.Scripts.Systems.Board.Contents.Obstacle.Overlay;
using Match3.Scripts.Systems.Board.Contents;
using UnityCoreModules.Services.ObjectPool;

namespace Match3.Scripts.Systems.Board.Systems
{
    public class PieceFactory : IPieceFactory
    {

        [System.Serializable]
        public class Config
        {
            public PiecePrefabDB PrefabDB;
            public Transform ContentContainer;
            public Transform OverlayContainer;
        }

        private readonly Config _config;

        #region Services
        private readonly GemSpriteProvider _gemSpriteProvider;
        private readonly IPoolManager _poolManager;
        #endregion


        #region Data Fields
        private readonly Dictionary<ObstacleType, GameObject> _obstaclePrefabDict = new();
        private readonly Dictionary<BoardPowerType, GameObject> _boardPowerPrefabDict = new();
        #endregion

        public PieceFactory(Config config, IPoolManager poolManager, GemSpriteProvider gemSpriteProvider)
        {
            _config = config;
            _gemSpriteProvider = ServiceLocator.Get<GemSpriteProvider>();
            _poolManager = ServiceLocator.Get<IPoolManager>();

            foreach (var mapping in _config.PrefabDB.ObstaclePrefabs)
            {
                _obstaclePrefabDict[mapping.Data.Type] = mapping.Prefab;
            }
            foreach (var mapping in _config.PrefabDB.BoardPowerPrefabs)
            {
                _boardPowerPrefabDict[mapping.Data.Type] = mapping.Prefab;
            }
        }

        #region Methods
        public void CreateContentDataForNode(TileNode node, TileSetupData setupData)
        {
            switch (setupData.contentType)
            {
                case PredefinedContentType.SpecificGem:
                    node.SetContent(new Gem(node, setupData.gemType));
                    break;
                case PredefinedContentType.BoardPower:
                    node.SetContent(new BoardPower(node, setupData.powerData));
                    break;
                case PredefinedContentType.ContentObstacle:
                    CreateObstacleData(node, setupData.contentObstacleData);
                    break;
            }

            if (setupData.overlayObstacleData != null)
            {
                CreateObstacleData(node, setupData.overlayObstacleData);
            }
        }

        public GameObject CreateVisualForNode(Board board, TileNode node, Vector3 startPosition)
        {
            GameObject contentGO = null;

            if (node.Content != null)
            {
                contentGO = CreateVisualForContent(board, node.Content, startPosition);
            }

            if (node.Overlay != null)
            {
                GameObject overlayGO = CreateVisualForContent(board, node.Overlay, startPosition);
                if (contentGO != null)
                {
                    overlayGO.transform.SetParent(contentGO.transform);
                    overlayGO.transform.localPosition = Vector3.zero;
                }
            }

            if (contentGO != null)
            {
                contentGO.name = $"{node.Content.GetType().Name} ({node.GridPosition.x},{node.GridPosition.y})";
            }

            return contentGO;
        }

        private GameObject CreateVisualForContent(Board board, object contentModel, Vector3 position)
        {
            switch (contentModel)
            {
                case Gem gem:
                    GameObject gemGO = _poolManager.Get(_config.PrefabDB.GenericGemPrefab);
                    gemGO.transform.SetParent(_config.ContentContainer);
                    gemGO.transform.position = position;
                    GemView gemView = gemGO.GetComponent<GemView>();
                    if (gemView != null)
                    {
                        gemView.SetSprite(_gemSpriteProvider.GetSprite(gem.Type));
                        board.RegisterView(gem, gemView);
                    }
                    return gemGO;

                case BoardPower power:
                    if (_boardPowerPrefabDict.TryGetValue(power.Data.Type, out GameObject powerPrefab))
                    {
                        GameObject powerGO = Object.Instantiate(powerPrefab, position, Quaternion.identity, _config.ContentContainer);
                        var powerView = powerGO.GetComponent<BoardPowerView>();
                        if (powerView != null)
                            board.RegisterView(power, powerView);
                        return powerGO;
                    }
                    break;

                case Obstacle obstacle:
                    if (_obstaclePrefabDict.TryGetValue(obstacle.Data.Type, out GameObject obstaclePrefab))
                    {
                        Transform parent = obstacle.Data.PlacementType == ObstaclePlacementType.Content ? _config.ContentContainer : _config.OverlayContainer;
                        GameObject obstacleGO = Object.Instantiate(obstaclePrefab, position, Quaternion.identity, parent);
                        var obstacleView = obstacleGO.GetComponent<ObstacleView>();
                        if (obstacleView != null)
                            obstacleView.Initialize(obstacle, _poolManager);
                        return obstacleGO;
                    }
                    break;
            }
            return null;
        }

        private void CreateObstacleData(TileNode node, ObstacleDataSO data)
        {
            if (data == null) return;

            Obstacle obstacleInstance = data.Type switch
            {
                ObstacleType.Crate => new Crate(node, data),
                ObstacleType.Ice => new Ice(node, data),
                _ => null,
            };

            if (obstacleInstance == null)
                return;

            if (data.PlacementType == ObstaclePlacementType.Content)
            {
                if (obstacleInstance is IBoardContent content)
                {
                    node.SetContent(content);
                }
                else
                {
                    Debug.LogWarning($"Obstacle '{data.Type}' is defined as 'Content' but its class does not implement IBoardContent.");
                }
            }
            else if (data.PlacementType == ObstaclePlacementType.Overlay)
            {
                if (obstacleInstance is IOverlay overlay)
                {
                    node.SetOverlay(overlay);
                }
                else
                {
                    Debug.LogWarning($"Obstacle '{data.Type}' is defined as 'Overlay' but its class does not implement IOverlay.");
                }
            }
        }
    }
    #endregion
}