using UnityEngine;

namespace Match3.Scripts.Systems.Board.Contents.Gem
{
    public class GemView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        public void SetSprite(Sprite sprite)
        {
            if (_renderer != null) _renderer.sprite = sprite;
        }
    }
}