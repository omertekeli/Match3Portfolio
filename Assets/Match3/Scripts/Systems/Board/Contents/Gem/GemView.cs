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
        private float _popAnimDuration = 0.3f;

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

        public override async UniTask PlayClearAnimation(float duration = 0.2F)
        {
            var currentPosition = gameObject.transform.position;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(Vector3.zero, _popAnimDuration).SetEase(Ease.InBack));
            sequence.Join(_renderer.DOFade(0f, _popAnimDuration));
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
    }
}