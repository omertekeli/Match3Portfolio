using Match3.Scripts.Systems.Board.Data;

namespace Match3.Scripts.Systems.Board.Contents.Obstacle.Overlay
{
    public class Ice : Obstacle, IOverlay
    {
        public Ice(TileNode node, ObstacleDataSO data) : base(node, data) { }

        public override void TakeDamage()
        {
            base.TakeDamage();
            if (CurrentHealth <= 0)
            {
                Node.ClearOverlay();
            }
        }

        public bool IsBlockingMatch() => CurrentHealth > 0;
        public bool IsBlockingSwap() => false;
    }
}