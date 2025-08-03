using UnityEngine;
using Cysharp.Threading.Tasks;
using Match3.Scripts.Systems.Level.Data;
using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Board.Data;
using System.Collections.Generic;

namespace Match3.Scripts.Systems.Board.Systems
{
    public class BoardFactory
    {
        [System.Serializable]
        public class Config
        {
            public GameObject CellBackgroundPrefab;
            public Transform BoardContainer;
            public Transform BackgroundContainer;
            public Transform ContentContainer;
            public Transform OverlayContainer;

            [Tooltip("Spacing between cells. 1 = no gap, 1.1 = 10% gap.")]
            public float CellSpacing = 1.05f;
            public IReadOnlyList<Transform> CleanableContainers =>
                new[] { BackgroundContainer, ContentContainer, OverlayContainer };

            [Tooltip("Final Board Position Offset.")]
            public Vector2 PositionOffset;
        }

        private readonly IPieceFactory _pieceFactory;
        private readonly Config _config;

        public BoardFactory(IPieceFactory pieceFactory, Config config)
        {
            _pieceFactory = pieceFactory;
            _config = config;
        }

        public async UniTask BuildBoardAsync(Board board, LevelDataSO levelData)
        {
            //Create the logical grid of TileNodes first.
            TileNode[,] grid = CreateGridData(board.Width, board.Height, levelData);
            board.SetGrid(grid);

            //Create all the visual GameObjects based on the data.
            await CreateVisualGridAsync(board, levelData);

            //Link neighbors now that all nodes exist in the grid.
            LinkNeighbors(board);

            //Scale and position the final board to fit the camera view.
            ScaleAndPositionBoard(board);

            // Note: The logic for the initial random gem fill would be called after this.
        }

        /// <summary>
        /// Creates the 2D array of TileNode data based on the LevelDataSO.
        /// </summary>
        private TileNode[,] CreateGridData(int width, int height, LevelDataSO levelData)
        {
            var grid = new TileNode[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    var setupData = levelData.TileSetups[index];

                    var behavior = CellBehaviorType.Normal;
                    if (setupData.groundType == InitialTileType.Hole)
                        behavior = CellBehaviorType.Hole;
                    else if (setupData.groundType == InitialTileType.Generator)
                        behavior = CellBehaviorType.Generator;

                    grid[x, y] = new TileNode(x, y, behavior);
                }
            }
            return grid;
        }

        /// <summary>
        /// Instantiates all the visual GameObjects (backgrounds, predefined content).
        /// </summary>
        private async UniTask CreateVisualGridAsync(Board board, LevelDataSO levelData)
        {
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    TileNode node = board[x, y];
                    Vector3 position = new Vector3(x * _config.CellSpacing, y * _config.CellSpacing, 0);

                    if (node.IsPlayable)
                    {
                        Object.Instantiate(_config.CellBackgroundPrefab, position, Quaternion.identity, _config.BackgroundContainer);
                    }

                    TileSetupData setupData = levelData.TileSetups[y * board.Width + x];
                    SpawnPiece(node, setupData);
                }
            }
            await UniTask.Yield();
        }

        private void SpawnPiece(TileNode node, TileSetupData setupData)
        {
            Vector3 contentPosition = new Vector3(node.GridPosition.x * _config.CellSpacing, node.GridPosition.y * _config.CellSpacing, 0);
            switch (setupData.contentType)
            {
                case PredefinedContentType.SpecificGem:
                    _pieceFactory.CreateGem(node, contentPosition, setupData.gemType);
                    break;
                case PredefinedContentType.BoardPower:
                    _pieceFactory.CreateBoardPower(node, contentPosition, setupData.powerData);
                    break;
                case PredefinedContentType.ContentObstacle:
                    _pieceFactory.CreateObstacle(node, contentPosition, setupData.contentObstacleData);
                    break;
            }
            if (setupData.overlayObstacleData != null)
            {
                _pieceFactory.CreateObstacle(node, contentPosition, setupData.overlayObstacleData);
            }
        }

        /// <summary>
        /// Sets the neighbor references for each playable TileNode for fast traversal.
        /// </summary>
        private void LinkNeighbors(Board board)
        {
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    var node = board[x, y];
                    if (node == null || !node.IsPlayable) continue;

                    node.Neighbors[Direction.Up] = board[x, y + 1];
                    node.Neighbors[Direction.Down] = board[x, y - 1];
                    node.Neighbors[Direction.Left] = board[x - 1, y];
                    node.Neighbors[Direction.Right] = board[x + 1, y];
                }
            }
        }

        /// <summary>
        /// Scales and centers the entire board container to fit a predefined safe area.
        /// </summary>
        private void ScaleAndPositionBoard(Board board)
        {
            // These values define the target area in world units your board should fit into.
            float safeAreaWidth = 7.0f;
            float safeAreaHeight = 11.0f;

            const float cellSize = 1f;

            // 1. Calculate the TRUE edge-to-edge native size of the board.
            // This is the span of the centers, PLUS half a cell on each end (which equals one full cell size).
            float totalNativeWidth = (board.Width > 1) ? (board.Width - 1) * _config.CellSpacing + cellSize : cellSize;
            float totalNativeHeight = (board.Height > 1) ? (board.Height - 1) * _config.CellSpacing + cellSize : cellSize;

            // 2. Calculate the scale factor based on this true size. This will now be < 1 for larger boards.
            float scaleX = (totalNativeWidth > 0.01f) ? safeAreaWidth / totalNativeWidth : 1f;
            float scaleY = (totalNativeHeight > 0.01f) ? safeAreaHeight / totalNativeHeight : 1f;
            float scaleFactor = Mathf.Min(scaleX, scaleY);

            _config.BoardContainer.localScale = new Vector3(scaleFactor, scaleFactor, 1f);

            // 3. Center the board. The logic for centering remains the same, but it now uses the
            //    correct native size measurement from the center of the first to the center of the last cell.
            float pivotSpanWidth = (board.Width > 1) ? (board.Width - 1) * _config.CellSpacing : 0f;
            float pivotSpanHeight = (board.Height > 1) ? (board.Height - 1) * _config.CellSpacing : 0f;

            float centerX = -pivotSpanWidth / 2f * scaleFactor;
            float centerY = -pivotSpanHeight / 2f * scaleFactor;

            _config.BoardContainer.position = new Vector3(
                centerX + _config.PositionOffset.x,
                centerY + _config.PositionOffset.y,
                0
            );
        }
    }
}