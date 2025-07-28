using Match3.Scripts.Systems.Board.Data;

namespace Match3.Scripts.Systems.Board.Contents.BoardPower
{
    public interface IBoardPowerStrategy
    {
        void Execute(TileNode originNode);
    }
}