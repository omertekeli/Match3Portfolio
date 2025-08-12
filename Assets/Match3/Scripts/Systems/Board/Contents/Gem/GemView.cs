using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityCoreModules.Services.ObjectPool;
using UnityEngine;

namespace Match3.Scripts.Systems.Board.Contents.Gem
{
    public class GemView : PieceView
    {
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private GameObject _gemPopVfxPrefab;

        public void Initialize(Gem model, IPoolManager poolManager)
        {
            base.Initialize(model, poolManager);
        }

        public void SetSprite(Sprite sprite)
        {
            if (_renderer != null)
            {
                _renderer.sprite = sprite;
            }
        }

        public override async UniTask PlayClearAnimation(float duration = 0.3f)
        {
            var currentPosition = gameObject.transform.position;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(Vector3.zero, duration).SetEase(Ease.InBack));
            sequence.Join(_renderer.DOFade(0f, duration));
            PlayVFX(currentPosition);
            await sequence.ToUniTask();


            if (this != null && this.gameObject != null)
            {
                base.ResetView();
                if (_poolManager != null)
                    _poolManager.Return(gameObject);
                else
                    Debug.LogError("Pool Manager is not exist!");
            }
        }

        private void PlayVFX(Vector3 spawnPosition)
        {
            GameObject vfxInstance = _poolManager.Get(_gemPopVfxPrefab);
            vfxInstance.transform.position = spawnPosition;
            var particleSystem = vfxInstance.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();

            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("HoleTrigger"))
            {
                _renderer.color = new Color(1f, 1f, 1f, 0f);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("HoleTrigger"))
            {
                _renderer.color = Color.white;
            }
        }
    }
}