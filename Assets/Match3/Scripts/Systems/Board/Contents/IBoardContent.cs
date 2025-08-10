using Match3.Scripts.Systems.Board.Data;

namespace Match3.Scripts.Systems.Board.Contents
{
    /// <summary>
    /// Gem, BoardPower, Obstacle etc.
    /// </summary>
    public interface IBoardContent
    {
        TileNode Node { get; set; }
        bool CanBeSwapped();
    }
}