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
using UnityEngine.LightTransport;
using Match3.Scripts.Systems.Board;

public class RefillSystem
{
    private readonly Board _board;
    private readonly IPieceFactory _pieceFactory;
    private readonly LevelDataSO _levelData;
    private readonly float _cellSpacing;
    private const float FALL_DURATION = 0.25f;

    public RefillSystem(Board board, IPieceFactory pieceFactory, LevelDataSO levelData)
    {
        _board = board;
        _pieceFactory = pieceFactory;
        _levelData = levelData;
        _cellSpacing = board.BoardFactoryConfig.CellSpacing;
    }

    /// <summary>
    /// Fills all empty cells on the board until it becomes stable.
    /// Manages both gravity for existing pieces and spawning for new pieces in a loop.
    /// </summary>
    public async UniTask RefillBoardAsync()
    {
        while (true)
        {
            var animationTasks = new List<UniTask>();
            bool boardChanged = false;

            // --- AŞAMA 1: YERÇEKİMİ ---
            // Sütun bazında, aşağıdan yukarıya tarayarak mevcut taşları düşür.

            for (int x = 0; x < _board.Width; x++)
            {
                // 1. ADIM: Sütundaki tüm mevcut taşları GEÇİCİ bir listeye topla.
                var piecesInColumn = new List<IBoardContent>();
                for (int y = 0; y < _board.Height; y++)
                {
                    var node = _board[x, y];
                    if (node != null && node.IsPlayable && !node.IsEmpty)
                    {
                        piecesInColumn.Add(node.Content);
                        node.UpdateContent(null); // Hücreyi mantıksal olarak geçici olarak boşalt
                    }
                }

                // Eğer sütunda hiç düşecek taş yoksa, bir sonraki sütuna geç.
                if (piecesInColumn.Count == 0) continue;

                // 2. ADIM: Taşları, sütunun en altındaki OYNANABİLİR ilk yuvadan başlayarak geri dağıt.
                int currentPieceIndex = 0;
                for (int y = 0; y < _board.Height; y++)
                {
                    var node = _board[x, y];
                    // Sadece oynanabilir yuvaları dikkate al. Hole'lar otomatik olarak atlanır.
                    if (node == null || !node.IsPlayable) continue;

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

            // --- AŞAMA 2: YARATMA ---
            // Sütunların tepesindeki boşlukları yeni taşlarla doldur.
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
                            sequence.Join(gemGO.transform.DOScale(Vector3.one, FALL_DURATION * 1.5f).SetEase(Ease.OutBack));
                            sequence.Join(gemGO.transform.DOMove(endPosition, FALL_DURATION * 1.5f).SetEase(Ease.OutQuad));
                            animationTasks.Add(sequence.ToUniTask());
                        }
                        boardChanged = true;
                    }
                }
            }

            // Eğer bu turda HİÇBİR taş hareket etmediyse veya yaratılmadıysa, board stabil demektir. Döngüden çık.
            if (!boardChanged)
            {
                break;
            }

            // Bu turdaki tüm animasyonların bitmesini bekle ve bir sonraki tur için tekrar başa dön.
            await UniTask.WhenAll(animationTasks);
        }
    }

    private GemType GetRandomGemType()
    {
        var availableTypes = _levelData.AvailablePieceTypes;
        if (availableTypes == null || availableTypes.Count == 0) return default;
        return availableTypes[Random.Range(0, availableTypes.Count)];
    }
}