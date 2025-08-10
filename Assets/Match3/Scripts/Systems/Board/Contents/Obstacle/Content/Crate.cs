using Match3.Scripts.Systems.Board.Data;

namespace Match3.Scripts.Systems.Board.Contents.Obstacle.Content
{
    public class Crate : Obstacle, IBoardContent
    {
        public TileNode Node { get;  set; }
        public Crate(TileNode node, ObstacleDataSO data) : base(data)
        {
            Node = node;
        }

        public bool CanBeSwapped()
        {
            return false;
        }

        public override void TakeDamage()
        {
            base.TakeDamage();
            if (CurrentHealth <= 0)
            {
                Node.SetContent(null);
            }
        }
    }
}