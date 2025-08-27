using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Match3.Scripts.Core.Interfaces;
using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Board.Contents;
using Match3.Scripts.Systems.Board.Contents.Gem;
using Match3.Scripts.Systems.Board.Data;
using Match3.Scripts.Systems.Board.Events;
using Match3.Scripts.Systems.Level.Data;
using UnityCoreModules.Services.EventBus;
using UnityEngine;

namespace Match3.Scripts.Systems.Board.Systems
{
    public class MoveProcessorSystem
    {
        #region Fields
        private readonly Board _board;
        private readonly IEventPublisher _publisher;
        private readonly IAudioManager _audioManager;
        private readonly MatchFindingSystem _matchFinder;
        private readonly RefillSystem _refillSystem;
        #endregion

        public MoveProcessorSystem(Board board, IAudioManager audioManager, IEventPublisher publisher, IPieceFactory pieceFactory, LevelDataSO levelData)
        {
            _board = board;
            _publisher = publisher;
            _audioManager = audioManager;
            _matchFinder = new MatchFindingSystem(board);
            _refillSystem = new RefillSystem(board, pieceFactory, levelData);
        }

        #region Methods

        public async UniTask<bool> ProcessMoveAsync(TileNode from, TileNode to)
        {
            if (!from.IsSwappable || !to.IsSwappable)
                return false;

            await SwapPiecesAsync(from, to, 0.3f);

            bool matchFound = await ProcessMatchesAndRefills();
            if (!matchFound)
            {
                await SwapPiecesAsync(from, to, 0.2f);
                return false;
            }
            return true;
        }

        private async UniTask<bool> ProcessMatchesAndRefills()
        {
            bool matchFoundInLoop = false;

            while (true)
            {
                var matches = _matchFinder.FindAllMatches();
                if (matches.Count == 0)
                    break;

                matchFoundInLoop = true;

                await ClearAndProcessMatchesAsync(matches);
                await _refillSystem.RefillBoardAsync();
            }
            return matchFoundInLoop;
        }

        private async UniTask ClearAndProcessMatchesAsync(List<TileNode> matchedNodes)
        {
            var tasks = new List<UniTask>();

            // TODO: Overlay checking
            var clearedPieces = new Dictionary<GemType, int>();
            foreach (var node in matchedNodes)
            {
                if (node.Content != null)
                {
                    var view = _board.GetViewFromContent(node.Content);
                    if (view != null)
                    {
                        tasks.Add(view.PlayClearAnimation());
                    }

                    if (node.Content is Gem gem)
                    {
                        if (clearedPieces.ContainsKey(gem.Type))
                            clearedPieces[gem.Type]++;
                        else
                            clearedPieces[gem.Type] = 1;
                    }

                    Debug.Log($"Clear at position: {node.Content.Node.GridPosition}");
                    _board.UnregisterView(node.Content);
                    node.UpdateContent(null);
                    _audioManager.PlaySfx(Enums.SfxType.PieceMatch);
                }
            }

            _publisher.Fire(new PiecesCleared(clearedPieces));
            await UniTask.WhenAll(tasks);
        }

        private async UniTask SwapPiecesAsync(TileNode a, TileNode b, float duration = 0.3f)
        {
            var contentA = a.Content;
            var contentB = b.Content;

            if (contentA == null || contentB == null)
                return;

            PieceView viewA = _board.GetViewFromContent(contentA);
            PieceView viewB = _board.GetViewFromContent(contentB);

            if (viewA == null || viewB == null)
                return;

            Vector3 posA = viewA.transform.position;
            Vector3 posB = viewB.transform.position;

            Sequence swapSequence = DOTween.Sequence();
            swapSequence.Join(viewA.transform.DOMove(posB, duration).SetEase(Ease.OutQuad));
            swapSequence.Join(viewB.transform.DOMove(posA, duration).SetEase(Ease.OutQuad));
            await swapSequence.ToUniTask();

            a.UpdateContent(contentB);
            b.UpdateContent(contentA);
        }

        #endregion
    }
}