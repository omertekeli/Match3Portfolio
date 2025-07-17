using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Scripts.UI
{   
    public class FadeController : MonoBehaviour
    {
        #region Fields
        
        [SerializeField] private Image _fadeImage;
        [SerializeField] private float _fadeDuration = 0.5f;
        
        private bool _isFading;
        
        #endregion
        
        private void Awake()
        {
            ToggleFadeImage(false);
        }
        
        public void ToggleFadeImage(bool shouldEnable)
        {
            _fadeImage.enabled = shouldEnable;  
        }
        
        public async Task FadeToWhiteAsync()
        { 
            await StartFadeAsync(1f, 0f);
        }

        public async Task FadeToBlackAsync()
        {
            await StartFadeAsync(0f, 1f);
        }

        private async Task StartFadeAsync(float from, float to)
        {
            if (_isFading) return;
            _isFading = true;
            float elapsed = 0f;
            Color color = _fadeImage.color;
            while (elapsed < _fadeDuration)
            {
                await Task.Yield();
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / _fadeDuration); 
                color.a = Mathf.Lerp(from, to, t);          
                _fadeImage.color = color;                      
            }
            color.a = to;
            _fadeImage.color = color;
            _isFading = false;
        }
    }
}