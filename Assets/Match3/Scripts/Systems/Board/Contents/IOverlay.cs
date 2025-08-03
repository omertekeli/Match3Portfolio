namespace Match3.Scripts.Systems.Board.Contents
{
    /// <summary>
    /// Layers over the content (e.g. Chain, Ice).
    /// </summary>
    public interface IOverlay
    {
        bool IsBlockingMatch();
        bool IsBlockingSwap();
    }
}