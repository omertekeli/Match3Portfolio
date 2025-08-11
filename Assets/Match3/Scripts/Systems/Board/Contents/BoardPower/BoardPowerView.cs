using UnityCoreModules.Services.ObjectPool;
using UnityEngine;

namespace Match3.Scripts.Systems.Board.Contents.BoardPower
{
    public class BoardPowerView : PieceView
    {
        [SerializeField] private SpriteRenderer _renderer;

        public void Initialize(BoardPower model, IPoolManager poolManager)
        {
            base.Initialize(model, poolManager);
            _renderer.sprite = model.Data.Sprite;
        }
    }
}