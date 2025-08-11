using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityCoreModules.Services.ObjectPool;
using UnityEngine;

namespace Match3.Scripts.Systems.Board.Contents
{
    public abstract class PieceView : MonoBehaviour
    {
        public object Model { get; private set; }
        protected IPoolManager _poolManager { get; private set; }
        public virtual void Initialize(object model, IPoolManager poolManager)
        {
            this.Model = model;
            this._poolManager = poolManager;
        }

        public virtual async UniTask PlayClearAnimation(float duration = 0.2f)
        {
            await transform.DOScale(Vector3.zero, duration).ToUniTask();
            if (_poolManager == null)
            {
                Destroy(gameObject);
            }
            else
            {
                _poolManager.Return(gameObject);
            }
        }

        public virtual void ResetView()
        {
            this.transform.localScale = Vector3.one;
            this.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}