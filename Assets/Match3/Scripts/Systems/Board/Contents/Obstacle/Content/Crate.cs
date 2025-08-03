using Match3.Scripts.Systems.Board.Data;

namespace Match3.Scripts.Systems.Board.Contents.Obstacle.Content
{
    public class Crate : Obstacle, IBoardContent
    {
        public Crate(TileNode node, ObstacleDataSO data) : base(node, data) { }

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