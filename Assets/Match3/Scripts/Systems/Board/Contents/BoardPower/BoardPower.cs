namespace Match3.Scripts.Systems.Board.Contents.BoardPower
{
    public class BoardPower : IBoardContent {
        public BoardPowerDataSO Data { get; private set; }
        public BoardPower(BoardPowerDataSO data)
        {
            Data = data;
        }

        public bool CanBeSwapped()
        {
            return true;
        }
    }
}