using Match3.Scripts.Systems.Board.Data;

namespace Match3.Scripts.Systems.Board.Contents
{
    /// <summary>
    /// Layers over the content (e.g. Chain, Ice).
    /// </summary>
    public interface IOverlay
    {
         public TileNode Node { get; }
        bool IsBlockingMatch();
        bool IsBlockingSwap();
    }
}