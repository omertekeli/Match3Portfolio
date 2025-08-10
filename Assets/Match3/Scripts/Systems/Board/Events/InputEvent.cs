using Match3.Scripts.Systems.Board.Contents;
using UnityCoreModules.Services.EventBus;

namespace Match3.Scripts.Systems.Board.Events
{
    public struct PiecePressedEvent : IEvent
    {
        public readonly PieceView Piece;
        public PiecePressedEvent(PieceView piece)
        {
            Piece = piece;
        }
    }

    public struct PieceReleasedEvent : IEvent
    {
        public readonly PieceView Piece;
        public PieceReleasedEvent(PieceView piece)
        {
            Piece = piece;
        }
    }

    public struct PieceDraggedEvent : IEvent
    {
        public readonly PieceView Piece;
        public PieceDraggedEvent(PieceView piece)
        {
            Piece = piece;
        }
    }
}