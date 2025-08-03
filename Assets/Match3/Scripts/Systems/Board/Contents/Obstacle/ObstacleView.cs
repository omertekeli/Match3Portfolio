using Match3.Scripts.Systems.Board.Contents.Obstacle;
using UnityEngine;

namespace Match3.Scripts.Systems.Board.Contents.Obstacle
{
    public class ObstacleView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        private Obstacle _model;
        public void Initialize(Obstacle model)
        {
            _model = model;
            _renderer.sprite = _model.Data.Sprite;

            // Subscribe brain events
            // e.g. _model.OnHealthChanged += PlayShakeAnimation;
        }
    }
}