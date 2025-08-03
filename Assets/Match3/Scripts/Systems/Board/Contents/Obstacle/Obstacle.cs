using Match3.Scripts.Systems.Board.Data;

namespace Match3.Scripts.Systems.Board.Contents.Obstacle
{
   public abstract class Obstacle
{
    public ObstacleDataSO Data { get; protected set; }
    public int CurrentHealth { get; protected set; }
    protected TileNode Node { get; private set; }

    protected Obstacle(TileNode node, ObstacleDataSO data)
    {
        Node = node;
        Data = data;
        CurrentHealth = data.Health;
    }

    public virtual void TakeDamage()
    {
        if (CurrentHealth > 0)
        {
            CurrentHealth--;
        }
    }
}
}