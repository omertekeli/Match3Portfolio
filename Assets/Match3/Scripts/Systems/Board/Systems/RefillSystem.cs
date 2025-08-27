using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Match3.Scripts.Systems.Level.Data;
using Match3.Scripts.Systems.Board.Data;
using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Board.Contents.Gem;
using DG.Tweening;
using System.Linq;
using Match3.Scripts.Systems.Board.Contents;
using Match3.Scripts.Systems.Board;

public class RefillSystem
{
    #region Fields
    private readonly Board _board;
    private readonly IPieceFactory _pieceFactory;
    private readonly LevelDataSO _levelData;
    private readonly float _cellSpacing;
    private const float FALL_DURATION = 0.25f;
    #endregion

    public RefillSystem(Board board, IPieceFactory pieceFactory, LevelDataSO levelData)
    {
        _board = board;
        _pieceFactory = pieceFactory;
        _levelData = levelData;
        _cellSpacing = board.BoardFactoryConfig.CellSpacing;
    }

    #region Methods
    public async UniTask RefillBoardAsync()
    {
        while (true)
        {
            var animationTasks = new List<UniTask>();
            bool boardChanged = false;

            ApplyGravity(animationTasks);
            boardChanged = SpawnPieces(animationTasks, boardChanged);

            if (!boardChanged)
            {
                break;
            }

            await UniTask.WhenAll(animationTasks);
        }
    }

    private bool SpawnPieces(List<UniTask> animationTasks, bool boardChanged)
    {
        for (int x = 0; x < _board.Width; x++)
        {
            for (int y = 0; y < _board.Height; y++)
            {
                TileNode node = _board[x, y];
                if (node == null || !node.IsPlayable || !node.IsEmpty)
                    continue;

                bool isSpawnPoint = node.Behavior == CellBehaviorType.Generator ||
                                    (_board[x, y + 1] == null || !_board[x, y + 1].IsPlayable);

                if (isSpawnPoint)
                {
                    GemType randomType = GetRandomGemType();
                    Vector3 endPosition = _board.GetWorldPosition(x, y);
                    Vector3 startPosition = _board.GetWorldPosition(x, _board.Height - 0.5f);

                    node.UpdateContent(new Gem(node, randomType));
                    GameObject gemGO = _pieceFactory.CreateVisualForNode(_board, node, startPosition);

                    if (gemGO != null)
                    {
                        gemGO.transform.localScale = Vector3.zero;
                        Sequence sequence = DOTween.Sequence();
                        sequence.Append(gemGO.transform.DOScale(Vector3.one, FALL_DURATION * 1f).SetEase(Ease.OutBack));
                        sequence.Join(gemGO.transform.DOMove(endPosition, FALL_DURATION * 1f).SetEase(Ease.OutQuad));
                        animationTasks.Add(sequence.ToUniTask());
                    }
                    boardChanged = true;
                }
            }
        }

        return boardChanged;
    }

    private void ApplyGravity(List<UniTask> animationTasks)
    {
        for (int x = 0; x < _board.Width; x++)
        {

            var piecesInColumn = new List<IBoardContent>();
            for (int y = 0; y < _board.Height; y++)
            {
                var node = _board[x, y];
                if (node != null && node.IsPlayable && !node.IsEmpty)
                {
                    piecesInColumn.Add(node.Content);
                    node.UpdateContent(null);
                }
            }


            if (piecesInColumn.Count == 0)
                continue;

            int currentPieceIndex = 0;
            for (int y = 0; y < _board.Height; y++)
            {
                var node = _board[x, y];

                if (node == null || !node.IsPlayable)
                    continue;

                if (currentPieceIndex < piecesInColumn.Count)
                {
                    var pieceToMove = piecesInColumn[currentPieceIndex];
                    node.UpdateContent(pieceToMove);

                    PieceView viewToMove = _board.GetViewFromContent(pieceToMove);
                    if (viewToMove != null)
                    {
                        Vector3 endPosition = new Vector3(x * _cellSpacing, y * _cellSpacing, 0);
                        animationTasks.Add(viewToMove.transform.DOLocalMove(endPosition, FALL_DURATION).SetEase(Ease.InQuad).ToUniTask());
                    }
                    currentPieceIndex++;
                }
            }
        }
    }

    private GemType GetRandomGemType()
    {
        var availableTypes = _levelData.AvailablePieceTypes;
        if (availableTypes == null || availableTypes.Count == 0) return default;
        return availableTypes[Random.Range(0, availableTypes.Count)];
    }
    #endregion
}