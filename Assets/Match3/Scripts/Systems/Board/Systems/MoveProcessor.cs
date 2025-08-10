using Cysharp.Threading.Tasks;
using DG.Tweening;
using Match3.Scripts.Systems.Board.Contents;
using Match3.Scripts.Systems.Board.Data;
using UnityCoreModules.Services.EventBus;
using UnityEngine;

namespace Match3.Scripts.Systems.Board.Systems
{
    public class MoveProcessor
    {
        #region Fields
        private readonly Board _board;
        private readonly IEventPublisher _publisher;
        #endregion

        public MoveProcessor(Board board, IEventPublisher publisher)
        {
            _board = board;
            _publisher = publisher;
        }

        #region Methods

        public async UniTask<bool> ProcessMoveAsync(TileNode from, TileNode to)
        {
            await SwapPiecesAsync(from, to);

            return true;
        }

        private async UniTask SwapPiecesAsync(TileNode a, TileNode b)
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
            float duration = 0.3f;

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