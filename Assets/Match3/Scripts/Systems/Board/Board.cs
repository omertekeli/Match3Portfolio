using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Match3.Scripts.Systems.Board.Contents;
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
        private List<GameObject> _visualsToAnimate;
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
            _pieceFactory = new PieceFactory(_piecePrefabDB, _boardFactoryConfig.ContentContainer, _boardFactoryConfig.OverlayContainer);
            _boardFactory = new BoardFactory(_pieceFactory, _boardFactoryConfig);
            _visualsToAnimate = new();
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
        public void CreateBoard(LevelDataSO levelData)
        {
            _currentState = BoardState.Initializing;

            CleanupBoard();

            this.Width = levelData.Width;
            this.Height = levelData.Height;

            _visualsToAnimate = _boardFactory.BuildBoard(this, levelData);
        }

        public async UniTask PlayIntroAnimationAsync()
        {
            if (_visualsToAnimate == null || _visualsToAnimate.Count == 0)
            {
                _currentState = BoardState.Idle;
                return;
            }

            var animationTasks = new List<UniTask>();
            float fallDuration = 0.6f;
            float rowDelay = 0.1f;

            foreach (GameObject pieceGO in _visualsToAnimate)
            {
                // Bu, 'BuildBoard'da 'startPosition' olarak ayarlanan mevcut pozisyondur.
                var view = pieceGO.GetComponent<PieceView>();
                var model = view.Model;
                TileNode node = FindNodeForModel(model);
                if (node == null)
                    continue;

                var renderer = pieceGO.GetComponentInChildren<SpriteRenderer>();
                if (renderer == null)
                    continue;

                Vector3 endPosition = GetWorldPosition(node.GridPosition.x, node.GridPosition.y);

                Sequence sequence = DOTween.Sequence();

                float delay = (node.GridPosition.y) * rowDelay;
                sequence.SetDelay(delay);
                sequence.Join(pieceGO.transform.DOMove(endPosition, fallDuration).SetEase(Ease.OutBounce));
                sequence.Join(renderer.DOFade(1f, fallDuration * 0.25f));

                var sequenceTask = sequence.ToUniTask();
                animationTasks.Add(sequenceTask);
            }

            await UniTask.WhenAll(animationTasks);

            _visualsToAnimate.Clear();
            _currentState = BoardState.Idle;
        }

        public void SetGrid(TileNode[,] newGrid) => _grid = newGrid;

        private Vector3 GetWorldPosition(int x, int y)
        {
            Vector3 localPos = new Vector3(x * _boardFactoryConfig.CellSpacing, y * _boardFactoryConfig.CellSpacing, 0);
            if (_boardFactoryConfig.BoardContainer != null)
            {
                return _boardFactoryConfig.BoardContainer.TransformPoint(localPos);
            }
            return localPos;
        }

        private TileNode FindNodeForModel(object model)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (this[x, y]?.Content == model || this[x, y]?.Overlay == model)
                    {
                        return this[x, y];
                    }
                }
            }
            return null;
        }

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