using System.Threading.Tasks;
using Match3.Scripts.UI.Base;
using UnityEngine;
using UnityEngine.TextCore;
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

        internal async Task FadeToAlpha(float targetAlpha)
        {
            SetVisible(true);
            SetImageVisibility(true);

            Color color = _fadeImage.color;
            float startAlpha = color.a;
            float time = 0f;

            while (time < _fadeDuration)
            {
                time += Time.deltaTime;
                float t = time / _fadeDuration;
                color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
                _fadeImage.color = color;
                await Task.Yield();
            }

            color.a = targetAlpha;
            _fadeImage.color = color;

            if (targetAlpha == 0)
            {
                SetVisible(false);
                SetImageVisibility(false);
            }

        }
        private void SetImageVisibility(bool isVisible) => _fadeImage.gameObject.SetActive(isVisible);
    }
}