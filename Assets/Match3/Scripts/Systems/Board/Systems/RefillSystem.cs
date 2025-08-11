using Cysharp.Threading.Tasks;
using Match3.Scripts.Systems.Level.Data;

namespace Match3.Scripts.Systems.Board.Systems
{
    public class RefillSystem
    {
        #region Fields
        private readonly Board _board;
        private readonly IPieceFactory _pieceFactory;
        private readonly LevelDataSO _levelData;
        #endregion

        public RefillSystem(Board board, IPieceFactory pieceFactory, LevelDataSO levelData)
        {
            _board = board;
            _pieceFactory = pieceFactory;
            _levelData = levelData;
        }

        #region Methods
        public async UniTask RefillBoardAsync()
        {
            await ApplyGravityAsync();
            await FillTopGapsAsync();
        }

        private async UniTask ApplyGravityAsync()
        {
            await UniTask.Yield(); // Placeholder
        }

        private async UniTask FillTopGapsAsync()
        {
            await UniTask.Yield(); // Placeholder
        }
        #endregion
    }
}