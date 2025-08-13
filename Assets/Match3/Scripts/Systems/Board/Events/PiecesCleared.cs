using System.Collections.Generic;
using Match3.Scripts.Enums;
using UnityCoreModules.Services.EventBus;

namespace Match3.Scripts.Systems.Board.Events
{
    public struct PiecesCleared : IEvent
    {
        public readonly Dictionary<GemType, int> ClearedPieces;
        public PiecesCleared(Dictionary<GemType, int> clearedPieces)
        {
            ClearedPieces = clearedPieces;
        }
    }
}