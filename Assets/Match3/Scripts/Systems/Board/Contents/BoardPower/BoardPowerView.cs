using UnityEngine;

namespace Match3.Scripts.Systems.Board.Contents.BoardPower
{
    public class BoardPowerView : PieceView
    {
        [SerializeField] private SpriteRenderer _renderer;

        public void Initialize(BoardPower model)
        {
            base.Initialize(model);
            _renderer.sprite = model.Data.Sprite;
        }
    }
}