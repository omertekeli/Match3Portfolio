using Match3.Scripts.Systems.Board.Data;

namespace Match3.Scripts.Systems.Board.Contents.Obstacle
{
   public abstract class Obstacle
{
    public ObstacleDataSO Data { get; protected set; }
    public int CurrentHealth { get; protected set; }

    protected Obstacle(ObstacleDataSO data)
    {
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