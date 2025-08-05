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

namespace Match3.Scripts.Systems.Board.Systems
{
    public class PieceFactory : IPieceFactory
    {
        #region Fields
        private readonly PiecePrefabDB _prefabDB;
        private readonly Transform _contentContainer;
        private readonly Transform _overlayContainer;
        private readonly GemSpriteProvider _gemSpriteProvider;

        private readonly Dictionary<ObstacleType, GameObject> _obstaclePrefabDict = new();
        private readonly Dictionary<BoardPowerType, GameObject> _boardPowerPrefabDict = new();
        #endregion

        public PieceFactory(PiecePrefabDB prefabDB, Transform contentContainer, Transform overlayContainer)
        {
            _prefabDB = prefabDB;
            _contentContainer = contentContainer;
            _overlayContainer = overlayContainer;
            _gemSpriteProvider = ServiceLocator.Get<GemSpriteProvider>();

            foreach (var mapping in _prefabDB.ObstaclePrefabs)
            {
                _obstaclePrefabDict[mapping.Data.Type] = mapping.Prefab;
            }
            foreach (var mapping in _prefabDB.BoardPowerPrefabs)
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
                    node.SetContent(new Gem(setupData.gemType));
                    break;
                case PredefinedContentType.BoardPower:
                    node.SetContent(new BoardPower(setupData.powerData));
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

        public GameObject CreateVisualForNode(TileNode node, Vector3 startPosition)
        {
            GameObject contentGO = null;

            if (node.Content != null)
            {
                contentGO = CreateVisualForContent(node.Content, startPosition);
            }

            if (node.Overlay != null)
            {
                GameObject overlayGO = CreateVisualForContent(node.Overlay, startPosition);
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

        private GameObject CreateVisualForContent(object contentModel, Vector3 position)
        {
            switch (contentModel)
            {
                case Gem gem:
                    GameObject gemGO = Object.Instantiate(_prefabDB.GenericGemPrefab, position, Quaternion.identity, _contentContainer);
                    GemView gemView = gemGO.GetComponent<GemView>();
                    if (gemView != null)
                    {
                        gemView.SetSprite(_gemSpriteProvider.GetSprite(gem.Type));
                        gemView.Initialize(gem);
                    }
                    return gemGO;

                case BoardPower power:
                    if (_boardPowerPrefabDict.TryGetValue(power.Data.Type, out GameObject powerPrefab))
                    {
                        GameObject powerGO = Object.Instantiate(powerPrefab, position, Quaternion.identity, _contentContainer);
                        var powerView = powerGO.GetComponent<BoardPowerView>();
                        if (powerView != null)
                            powerView.Initialize(power);
                        return powerGO;
                    }
                    break;

                case Obstacle obstacle:
                    if (_obstaclePrefabDict.TryGetValue(obstacle.Data.Type, out GameObject obstaclePrefab))
                    {
                        Transform parent = obstacle.Data.PlacementType == ObstaclePlacementType.Content ? _contentContainer : _overlayContainer;
                        GameObject obstacleGO = Object.Instantiate(obstaclePrefab, position, Quaternion.identity, parent);
                        var obstacleView = obstacleGO.GetComponent<ObstacleView>();
                        if (obstacleView != null)
                            obstacleView.Initialize(obstacle);
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