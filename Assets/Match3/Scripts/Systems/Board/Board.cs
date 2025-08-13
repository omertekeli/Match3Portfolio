using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Match3.Scripts.Configs;
using Match3.Scripts.Core;
using Match3.Scripts.Core.Events;
using Match3.Scripts.Core.Interfaces;
using Match3.Scripts.Systems.Board.Contents;
using Match3.Scripts.Systems.Board.Contents.Gem;
using Match3.Scripts.Systems.Board.Data;
using Match3.Scripts.Systems.Board.Systems;
using Match3.Scripts.Systems.Level.Data;
using NUnit.Framework;
using UnityCoreModules.Services;
using UnityCoreModules.Services.EventBus;
using UnityCoreModules.Services.ObjectPool;
using UnityEngine;

namespace Match3.Scripts.Systems.Board
{
    public class Board : MonoBehaviour
    {
        #region Inspector References
        [Header("Factory Configuration")]
        [Tooltip("Settings for the BoardFactory that constructs the board.")]
        [SerializeField] private PieceFactory.Config _pieceFactoryConfig;
        [SerializeField] private BoardFactory.Config _boardFactoryConfig;
        #endregion

        #region State
        private enum BoardState { Initializing, Idle, Busy }
        private BoardState _currentState;
        #endregion

        #region Data Fields
        private TileNode[,] _grid;
        private readonly Dictionary<IBoardContent, PieceView> _viewMap = new();
        private List<GameObject> _visualsToAnimate;
        #endregion

        #region Services
        private IPoolManager _poolManager;
        private GemSpriteProvider _gemSpriteProvider;
        #endregion

        #region Helper Class
        private BoardFactory _boardFactory;
        private IPieceFactory _pieceFactory;
        private MoveProcessorSystem _moveProcessor;
        #endregion

        #region Properties
        public int Width { get; private set; }
        public int Height { get; private set; }
        public BoardFactory.Config BoardFactoryConfig => _boardFactoryConfig;

        public TileNode this[int x, int y] => (x >= 0 && x < Width && y >= 0 && y < Height) ? _grid[x, y] : null;
        public TileNode this[Vector2Int coords] => this[coords.x, coords.y];
        #endregion

        #region Unity Methods
        private void Awake()
        {
            _poolManager = ServiceLocator.Get<IPoolManager>();
            _gemSpriteProvider = ServiceLocator.Get<GemSpriteProvider>();
            _pieceFactory = new PieceFactory(_pieceFactoryConfig, _poolManager, _gemSpriteProvider);
            _boardFactory = new BoardFactory(_pieceFactory, _boardFactoryConfig);
            _visualsToAnimate = new();
        }
        private void OnEnable()
        {
            ServiceLocator.Get<IInputSystem>().SwapRequested += OnSwapRequested;
        }

        private void OnDisable()
        {
            ServiceLocator.Get<IInputSystem>().SwapRequested -= OnSwapRequested;
        }
        #endregion

        #region  Public Methods
        public void CreateBoard(LevelDataSO levelData)
        {
            _currentState = BoardState.Initializing;

            CleanupBoard();

            this.Width = levelData.Width;
            this.Height = levelData.Height;

            _visualsToAnimate = _boardFactory.BuildBoard(this, levelData);
            _moveProcessor = new MoveProcessorSystem(this, ServiceLocator.Get<IEventPublisher>(), _pieceFactory, levelData);
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
                var view = pieceGO.GetComponent<PieceView>();
                var model = view.Model;
                TileNode node = FindNodeFromModel(model);
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

        public PieceView GetViewFromContent(IBoardContent model)
        {
            return _viewMap.TryGetValue(model, out var view) ? view : null;
        }

        public void RegisterView(IBoardContent model, PieceView view)
        {
            if (model == null || view == null) return;
            _viewMap[model] = view;
            view.Initialize(model, _poolManager);
        }

        public void UnregisterView(IBoardContent model)
        {
            if (model != null && _viewMap.ContainsKey(model))
            {
                _viewMap.Remove(model);
            }
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            Vector3 localPos = new Vector3(x * _boardFactoryConfig.CellSpacing, y * _boardFactoryConfig.CellSpacing, 0);
            if (_boardFactoryConfig.BoardContainer != null)
            {
                return _boardFactoryConfig.BoardContainer.TransformPoint(localPos);
            }
            return localPos;
        }

        public Vector3 GetWorldPosition(float x, float y)
        {
            Vector3 localPos = new Vector3(x * _boardFactoryConfig.CellSpacing, y * _boardFactoryConfig.CellSpacing, 0);
            if (_boardFactoryConfig.BoardContainer != null)
            {
                return _boardFactoryConfig.BoardContainer.TransformPoint(localPos);
            }
            return localPos;
        }
        #endregion

        #region Private Methods
        private TileNode FindNodeFromModel(object model)
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

        private async void OnSwapRequested(SwapRequested args)
        {
            if (_currentState != BoardState.Idle)
                return;

            ServiceLocator.Get<ILevelManager>().DecrementMove();

            bool moveWasSuccessful = false;
            try
            {
                _currentState = BoardState.Busy;
                moveWasSuccessful = await SwapAsync(args);
            }
            finally
            {
                _currentState = BoardState.Idle;
                if (moveWasSuccessful)
                {
                    //ServiceLocator.Get<ILevelManager>().DecrementMove();
                }
            }
        }

        private async UniTask<bool> SwapAsync(SwapRequested args)
        {
            var successful = await _moveProcessor.ProcessMoveAsync(args.StartNode, args.EndNode);
            return successful;
        }
        #endregion
    }
}