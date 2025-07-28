using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Match3.Scripts.Systems.Board.Contents.Gem
{
    public class GemView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        public void SetSprite(Sprite sprite) => _spriteRenderer.sprite = sprite;

        public async UniTask PlayMatchAnimationAsync()
        {
            
        }

        public async UniTask PlaySpawnAnimation()
        {

        }
    }
}