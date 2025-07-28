namespace Match3.Scripts.Systems.Board.Contents.Obstacle
{
    public struct ObstacleData
    {
        public ObstacleType Type { get; }
        public int HitPoints { get; }
        public bool BlocksGemFall { get; }
        public bool IsGoal { get; }

        public ObstacleData(ObstacleType type, int hitPoints = 1, bool blocksGemFall = true, bool isGoal = false)
        {
            Type = type;
            HitPoints = hitPoints;
            BlocksGemFall = blocksGemFall;
            IsGoal = isGoal;
        }
    }
}