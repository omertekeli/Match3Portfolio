using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Match3.Scripts.Systems.Board.Data;
using Match3.Scripts.Systems.Level.Data;
using UnityEngine;

namespace Match3.Scripts.Systems.Board
{
    public class Board : MonoBehaviour
    {
        #region Fields

        private enum BoardState { Initializing, Idle, Busy }
        private BoardState _currentState;
        private TileNode[,] _grid;
        private List<TileNode> _playableNodes;

        //private MoveProcessorSystem _moveProcessor;
        //private RefillSystem _refillSystem;

        #endregion

        #region Properties
        public int Width { get; private set; }
        public int Height { get; private set; }
        public TileNode this[int x, int y] => (x >= 0 && x < Width && y >= 0 && y < Height) ? _grid[x, y] : null;
        public TileNode this[Vector2Int coords] => this[coords.x, coords.y];

        #endregion

        #region Unity Methods
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
        public async UniTask CreateBoard(LevelDataSO levelData)
        {
            _currentState = BoardState.Initializing;

            // Width, Height, _grid, _playableNodes init setup

            // Fill gems
            // await _refillSystem.InitialFillAsync();

            _currentState = BoardState.Idle;
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