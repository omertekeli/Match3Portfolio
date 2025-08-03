using UnityEngine;

namespace Match3.Scripts.Systems.Board.Contents.Obstacle
{
    public enum ObstacleType { None, Ice, Chain, Crate, Rock, DoubleIce, Lock, Vines, PortalBlocker }
    public enum ObstaclePlacementType { Content, Overlay }

    [CreateAssetMenu(fileName = "Obstacle_", menuName = "Match3/Obstacle Data")]
    public class ObstacleDataSO : ScriptableObject
    {
        [Header("General")]
        [SerializeField] private ObstacleType _type;
        [SerializeField] private ObstaclePlacementType _placementType;

        [Header("View")]
        [SerializeField] private Sprite _sprite;

        [Header("Behaviour")]
        [SerializeField] private bool _isbreakable;
        [SerializeField] private int _health = 1;

        public ObstacleType Type => _type;
        public ObstaclePlacementType PlacementType => _placementType;
        public Sprite Sprite => _sprite;
        public bool IsBreakable => _isbreakable;
        public int Health => _health;
    }
}