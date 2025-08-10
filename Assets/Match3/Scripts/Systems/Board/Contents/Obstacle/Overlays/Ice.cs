using Match3.Scripts.Systems.Board.Data;
using UnityEditor.Experimental.GraphView;

namespace Match3.Scripts.Systems.Board.Contents.Obstacle.Overlay
{
    public class Ice : Obstacle, IOverlay
    {
        public TileNode Node { get; private set; }

        public Ice(TileNode node, ObstacleDataSO data) : base(data)
        {
            Node = node;
        }

        public override void TakeDamage()
        {
            base.TakeDamage();
            if (CurrentHealth <= 0)
            {
                Node.ClearOverlay();
            }
        }

        public bool IsBlockingMatch() => CurrentHealth > 0;
        public bool IsBlockingSwap() => CurrentHealth > 0;
    }
}