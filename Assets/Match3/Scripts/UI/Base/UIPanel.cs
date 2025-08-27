using Cysharp.Threading.Tasks;
using DG.Tweening;
using Match3.Scripts.Enums;
using UnityEngine;

namespace Match3.Scripts.UI.Base
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIPanel : MonoBehaviour
    {
        protected CanvasGroup _canvasGroup;
        public abstract PopupType Type { get; }

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual async UniTask ShowAsync(float duration = 0.3f)
        {
            _canvasGroup.alpha = 0f;
            gameObject.SetActive(true);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            await _canvasGroup.DOFade(1f, duration).SetEase(Ease.OutQuad).ToUniTask();
        }

        public virtual async UniTask HideAsync(float duration = 0.3f)
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            await _canvasGroup.DOFade(0f, duration).SetEase(Ease.InQuad).ToUniTask();
            gameObject.SetActive(false);
        }
    }
}