using UnityEngine;
using Cysharp.Threading.Tasks;
using Match3.Scripts.Systems.Level.Data;
using Match3.Scripts.Enums;
using System.Collections.Generic;
using System.Linq;
using Match3.Scripts.Systems.Board.Data;
using Match3.Scripts.Systems.Board.Contents.Gem;
using DG.Tweening;
using Match3.Scripts.Systems.Board.Contents.BoardPower;

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

            [Tooltip("Final position offset to fine-tune the board's placement.")]
            public Vector2 PositionOffset;

            public IReadOnlyList<Transform> CleanableContainers =>
                new[] { BackgroundContainer, ContentContainer, OverlayContainer };
        }

        #region Fields
        private readonly IPieceFactory _pieceFactory;
        private readonly Config _config;
        private Dictionary<TileNode, GameObject> _visualMap;
        #endregion

        public BoardFactory(IPieceFactory pieceFactory, Config config)
        {
            _pieceFactory = pieceFactory;
            _config = config;
            _visualMap = new Dictionary<TileNode, GameObject>();
        }

        public List<GameObject> BuildBoard(Board board, LevelDataSO levelData)
        {
            TileNode[,] grid = CreateAndPopulateGridData(board, levelData);
            board.SetGrid(grid);
            LinkNeighbors(board);
            var visualsToAnimate = CreateVisuals(board);
            ScaleAndPositionBoard(board);
            return visualsToAnimate;
        }

        /// <summary>
        /// Creates the logical grid and populates it with all content data (predefined and random).
        /// </summary>
        private TileNode[,] CreateAndPopulateGridData(Board board, LevelDataSO levelData)
        {
            int width = board.Width;
            int height = board.Height;
            var grid = new TileNode[width, height];

            // Create all the base TileNodes.
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var setupData = levelData.TileSetups[y * width + x];
                    var behavior = setupData.groundType switch
                    {
                        InitialTileType.Hole => CellBehaviorType.Hole,
                        InitialTileType.Generator => CellBehaviorType.Generator,
                        _ => CellBehaviorType.Normal,
                    };
                    grid[x, y] = new TileNode(x, y, behavior);
                }
            }

            // Populate with content data (both predefined and random).
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    TileNode node = grid[x, y];
                    if (node == null || !node.IsPlayable)
                        continue;

                    var setupData = levelData.TileSetups[y * width + x];

                    if (setupData.contentType != PredefinedContentType.RandomGem)
                    {
                        _pieceFactory.CreateContentDataForNode(node, setupData);
                    }
                    else
                    {
                        GemType safeType = GetSafeRandomGemType(grid, x, y, width, levelData);
                        node.SetContent(new Gem(node, safeType));
                    }
                }
            }
            return grid;
        }

        /// <summary>
        /// Instantiates all visuals based on the populated grid data
        /// </summary>
        private List<GameObject> CreateVisuals(Board board)
        {
            var visualsToAnimate = new List<GameObject>();
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    TileNode node = board[x, y];
                    if (node == null)
                        continue;
                    if (!node.IsPlayable)
                        continue;

                    Vector3 targetPosition = new Vector3(x * _config.CellSpacing, y * _config.CellSpacing, 0);
                    GameObject cellGO = Object.Instantiate(_config.CellBackgroundPrefab, targetPosition, Quaternion.identity, _config.BackgroundContainer);
                    var cellView = cellGO.GetComponent<CellView>();
                    if (cellView != null)
                    {
                        cellView.Initialize(node.Behavior);
                    }

                    if (node.IsEmpty)
                        continue;

                    if (node.Content is Gem || node.Content is BoardPower)
                    {
                        Vector3 startPosition = new Vector3(x * _config.CellSpacing, (board.Height) * _config.CellSpacing, 0);
                        GameObject pieceGO = _pieceFactory.CreateVisualForNode(board, node, startPosition);
                        if (pieceGO)
                        {
                            var renderer = pieceGO.GetComponentInChildren<SpriteRenderer>();
                            if (renderer == null)
                                continue;
                            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0f);
                            visualsToAnimate.Add(pieceGO);
                        }
                    }
                    else
                    {
                        GameObject pieceGO = _pieceFactory.CreateVisualForNode(board, node, targetPosition);
                    }
                }
            }
            return visualsToAnimate;
        }

        /// <summary>
        /// Gets a random GemType that will not create a 3-match at the given coordinates.
        /// </summary>
        private GemType GetSafeRandomGemType(TileNode[,] grid, int x, int y, int width, LevelDataSO levelData)
        {
            var possibleTypes = new List<GemType>(levelData.AvailablePieceTypes);

            if (x > 1)
            {
                var gem1 = grid[x - 1, y]?.Content as Gem;
                var gem2 = grid[x - 2, y]?.Content as Gem;
                if (gem1 != null && gem2 != null && gem1.Type == gem2.Type)
                    possibleTypes.Remove(gem1.Type);
            }
            if (y > 1)
            {
                var gem1 = grid[x, y - 1]?.Content as Gem;
                var gem2 = grid[x, y - 2]?.Content as Gem;
                if (gem1 != null && gem2 != null && gem1.Type == gem2.Type)
                    possibleTypes.Remove(gem1.Type);
            }

            if (possibleTypes.Count == 0)
                return levelData.AvailablePieceTypes.FirstOrDefault();

            return possibleTypes[Random.Range(0, possibleTypes.Count)];
        }

        /// <summary>
        /// Sets the neighbor references for each playable TileNod
        /// </summary>
        private void LinkNeighbors(Board board)
        {
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    var node = board[x, y];
                    if (node == null || !node.IsPlayable)
                        continue;
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
            float safeAreaWidth = 7.0f;
            float safeAreaHeight = 11.0f;

            const float cellSize = 1f;

            float totalNativeWidth = (board.Width > 1) ? (board.Width - 1) * _config.CellSpacing + cellSize : cellSize;
            float totalNativeHeight = (board.Height > 1) ? (board.Height - 1) * _config.CellSpacing + cellSize : cellSize;

            float scaleX = (totalNativeWidth > 0.01f) ? safeAreaWidth / totalNativeWidth : 1f;
            float scaleY = (totalNativeHeight > 0.01f) ? safeAreaHeight / totalNativeHeight : 1f;
            float scaleFactor = Mathf.Min(scaleX, scaleY);

            _config.BoardContainer.localScale = new Vector3(scaleFactor, scaleFactor, 1f);

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