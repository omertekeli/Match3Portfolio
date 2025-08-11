using UnityCoreModules.Services.ObjectPool;
using UnityEngine;

namespace Match3.Scripts.Systems.Board.Contents.Obstacle
{
    public class ObstacleView : PieceView
    {
        [SerializeField] private SpriteRenderer _renderer;

        public void Initialize(Obstacle model, IPoolManager poolManager)
        {
            base.Initialize(model, poolManager);
            _renderer.sprite = model.Data.Sprite;
            // model.OnHealthChanged += OnHealthChanged;
        }

        private void OnHealthChanged(int newHealth)
        {
            
        }

        private void OnDestroy()
        {
            // var model = this.Model as Obstacle;
            // if(model != null) 
                //model.OnHealthChanged -= OnHealthChanged;
        }
    }
}