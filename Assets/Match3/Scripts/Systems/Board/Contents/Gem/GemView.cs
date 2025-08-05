using UnityEngine;

namespace Match3.Scripts.Systems.Board.Contents.Gem
{
    public class GemView : PieceView
    {
        [SerializeField] private SpriteRenderer _renderer;

        public void Initialize(Gem model)
        {
            base.Initialize(model);
        }

        public void SetSprite(Sprite sprite)
        {
            if (_renderer != null)
            {
                _renderer.sprite = sprite;
            }
        }
    }
}