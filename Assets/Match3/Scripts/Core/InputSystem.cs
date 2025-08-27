using System;
using Match3.Scripts.Core.Events;
using Match3.Scripts.Core.Interfaces;
using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Board.Contents;
using Match3.Scripts.Systems.Board.Data;
using Match3.Scripts.Systems.Board.Events;
using UnityCoreModules.Services;
using UnityCoreModules.Services.EventBus;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Match3.Scripts.Core
{
    public class InputSystem : IInputSystem
    {
        public event Action<SwapRequested> SwapRequested;
        private readonly IEventSubscriber _subscriber;
        private PieceView _pressedPiece;
        private Vector2 _pressPosition;
        private bool _isDragging;
        private const float _MinDragDistance = 30f;

        public InputSystem()
        {
            _subscriber = ServiceLocator.Get<IEventSubscriber>();
            _subscriber.Subscribe<PiecePressedEvent>(HandlePiecePressed);
            _subscriber.Subscribe<PieceReleasedEvent>(HandlePieceReleased);
        }

        public void Shutdown()
        {
            _subscriber.Unsubscribe<PiecePressedEvent>(HandlePiecePressed);
            _subscriber.Unsubscribe<PieceReleasedEvent>(HandlePieceReleased);
        }

        private void HandlePiecePressed(PiecePressedEvent eventData)
        {
            if (eventData.Piece == null)
                return;
            _pressedPiece = eventData.Piece;
            _pressPosition = Pointer.current.position.ReadValue();
            _isDragging = true;
        }

        private void HandlePieceReleased(PieceReleasedEvent eventData)
        {
            Debug.Log($"Current released piece: {eventData.Piece.name}");
            if (!_isDragging || _pressedPiece == null) return;

            _isDragging = false;

            Vector2 releasePosition = Pointer.current.position.ReadValue();
            Vector2 dragDelta = releasePosition - _pressPosition;

            if (dragDelta.magnitude < _MinDragDistance)
            {
                _pressedPiece = null;
                return;
            }

            Direction swipeDirection = GetSwipeDirection(dragDelta);
            if (swipeDirection == Direction.None)
            {
                _pressedPiece = null;
                return;
            }

            var fromNode = (_pressedPiece.Model as IBoardContent)?.Node;
            if (fromNode == null || !fromNode.Neighbors.TryGetValue(swipeDirection, out TileNode toNode) || toNode == null)
            {
                _pressedPiece = null;
                return;
            }

            var swapArgs = new SwapRequested(fromNode, toNode);
            SwapRequested?.Invoke(swapArgs);
            _pressedPiece = null;
        }

        private Direction GetSwipeDirection(Vector2 delta)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                return delta.x > 0 ? Direction.Right : Direction.Left;
            }
            else
            {
                return delta.y > 0 ? Direction.Up : Direction.Down;
            }
        }

    }
}