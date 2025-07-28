using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Match3.Scripts.UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Scripts.UI.Views
{
    public class FadeView : UIView
    {
        #region Fields

        [SerializeField] private Image _fadeImage;
        [SerializeField] private float _fadeDuration = 0.5f;

        #endregion

        public override void SetVisible(bool shouldVisible) => base.SetVisible(shouldVisible);

        internal async UniTask FadeToAlpha(float targetAlpha, float? duration = null, Ease ease = Ease.InOutQuad)
        {
            _fadeImage.DOKill();
            SetVisible(true);
            SetImageVisibility(true);

            float finalDuration = duration ?? _fadeDuration;
            await _fadeImage.DOFade(targetAlpha, finalDuration)
                .SetEase(ease)
                .OnComplete(() =>
                {
                    if (targetAlpha == 0)
                    {
                        SetVisible(false);
                        SetImageVisibility(false);
                    }
                });
        }
        private void SetImageVisibility(bool isVisible) => _fadeImage.gameObject.SetActive(isVisible);
    }
}