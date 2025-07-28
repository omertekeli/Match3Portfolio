using Match3.Scripts.Systems.Board.Data;

namespace Match3.Scripts.Systems.Board.Contents.BoardPower
{
    public class BoardPower
    {
        #region Properties

        public BoardPowerData Data { get; private set; }
        public BoardPowerView View { get; private set; }
        public IBoardPowerStrategy Strategy { get; private set; }

        #endregion

        #region Methods
        
        public void Bind(BoardPowerData data, BoardPowerView view, IBoardPowerStrategy strategy)
        {
            Data = data;
            View = view;
            Strategy = strategy;
        }

        public void Activate(TileNode node) => Strategy.Execute(node);

        #endregion
    }
}