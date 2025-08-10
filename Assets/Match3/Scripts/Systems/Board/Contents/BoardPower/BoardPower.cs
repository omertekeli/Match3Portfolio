using Match3.Scripts.Systems.Board.Data;

namespace Match3.Scripts.Systems.Board.Contents.BoardPower
{
    public class BoardPower : IBoardContent {
        public BoardPowerDataSO Data { get; private set; }
        public TileNode Node { get;  set; }

        public BoardPower(TileNode node, BoardPowerDataSO data)
        {
            Data = data;
            Node = node;
        }

        public bool CanBeSwapped()
        {
            return true;
        }
    }
}