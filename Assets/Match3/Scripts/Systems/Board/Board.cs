using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Match3.Scripts.Systems.Board.Data;
using Match3.Scripts.Systems.Board.Systems;
using Match3.Scripts.Systems.Level.Data;
using UnityEngine;

namespace Match3.Scripts.Systems.Board
{
    public class Board : MonoBehaviour
    {
        #region State
        private enum BoardState { Initializing, Idle, Busy }
        private BoardState _currentState;
        #endregion

        #region Inspector References
        [Header("Data & Prefabs")]
        [Tooltip("Reference to the ScriptableObject containing all piece prefabs.")]
        [SerializeField] private PiecePrefabDB _piecePrefabDB;

        [Header("Factory Configuration")]
        [Tooltip("Settings for the BoardFactory that constructs the board.")]
        [SerializeField] private BoardFactory.Config _boardFactoryConfig;
        #endregion

        #region Data Fields
        private TileNode[,] _grid;
        #endregion

        #region Properties
        public int Width { get; private set; }
        public int Height { get; private set; }

        public TileNode this[int x, int y] => (x >= 0 && x < Width && y >= 0 && y < Height) ? _grid[x, y] : null;
        public TileNode this[Vector2Int coords] => this[coords.x, coords.y];
        #endregion

        #region Helper Classes
        private BoardFactory _boardFactory;
        private IPieceFactory _pieceFactory;
        #endregion

        #region Unity Methods

        private void Awake()
        {
            _pieceFactory = new PieceFactory(_piecePrefabDB, _boardFactoryConfig.ContentContainer);
            _boardFactory = new BoardFactory(_pieceFactory, _boardFactoryConfig);
        }
        private void OnEnable()
        {
            // InputSystem.OnSwapIntent += OnSwapIntent;
        }

        private void OnDisable()
        {
            //InputSystem.OnSwapIntent -= OnSwapIntent;
        }
        #endregion

        #region Main Methods
        public async UniTask CreateBoardAsycnc(LevelDataSO levelData)
        {
            _currentState = BoardState.Initializing;

            CleanupBoard();

            this.Width = levelData.Width;
            this.Height = levelData.Height;

            await _boardFactory.BuildBoardAsync(this, levelData);

            // TODO: Call the Initial Fill logic here for empty tiles.
            // This will be handled by a RefillSystem.
            // await _refillSystem.InitialFillAsync(levelData);

            _currentState = BoardState.Idle;
        }

        /// <summary>
        /// A "setter" method for the factory to assign the created grid back to the Board.
        /// </summary>
        public void SetGrid(TileNode[,] newGrid)
        {
            _grid = newGrid;
        }

        /// <summary>
        /// Destroys all visual GameObjects from the previous board setup.
        /// </summary>
        private void CleanupBoard()
        {
            foreach (var container in _boardFactoryConfig.CleanableContainers)
            {
                foreach (var item in container)
                {
                    Destroy(container.gameObject);
                }
            }
        }


        /*        
        private async void OnSwapIntent(SwapIntentArgs args)
        {
            if (_currentState != BoardState.Idle) return;

            try
            {
                _currentState = BoardState.Busy;
                await _moveProcessor.ProcessMoveAsync(args.From, args.To);
            }
            finally
            {
                _currentState = BoardState.Idle;
            }
        }
        */
        #endregion
    }
}