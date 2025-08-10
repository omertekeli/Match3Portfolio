using Match3.Scripts.Systems.Board.Data;
using UnityCoreModules.Services.EventBus;

namespace Match3.Scripts.Core.Events
{
    public struct SwapRequested : IEvent
    {
        public readonly TileNode StartNode;
        public readonly TileNode EndNode;
        public SwapRequested(TileNode startNode, TileNode endNode)
        {
            StartNode = startNode;
            EndNode = endNode;
        }
    }
}