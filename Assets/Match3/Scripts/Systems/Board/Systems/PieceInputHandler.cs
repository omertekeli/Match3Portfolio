using Match3.Scripts.Systems.Board.Contents;
using Match3.Scripts.Systems.Board.Events;
using UnityCoreModules.Services;
using UnityCoreModules.Services.EventBus;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Match3.Scripts.Systems.Board.Systems
{
    public class PieceInputHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        private PieceView _pieceView;
        private IEventPublisher _publisher;

        private void Awake()
        {
            _pieceView = GetComponent<PieceView>();
            _publisher = ServiceLocator.Get<IEventPublisher>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _publisher.Fire(new PiecePressedEvent(_pieceView));
        }

        public void OnDrag(PointerEventData eventData)
        {
            _publisher.Fire(new PieceDraggedEvent(_pieceView));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _publisher.Fire(new PieceReleasedEvent(_pieceView));
        }
    }

}