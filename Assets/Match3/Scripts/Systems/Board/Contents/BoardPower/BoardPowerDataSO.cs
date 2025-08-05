using UnityEngine;

namespace Match3.Scripts.Systems.Board.Contents.BoardPower
{
    public enum BoardPowerType { Rocket, Bomb }

    [CreateAssetMenu(fileName = "Power_", menuName = "Match3/Board Power Data")]
    public class BoardPowerDataSO : ScriptableObject
    {
        [Header("General")]
        [SerializeField] private BoardPowerType _type;

        [Header("View")]
        [SerializeField] private Sprite _sprite;
        
        public BoardPowerType Type => _type;
        public Sprite Sprite => _sprite;
    }
}