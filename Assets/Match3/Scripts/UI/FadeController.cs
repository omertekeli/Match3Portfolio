using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Scripts.UI
{   
    public class FadeController : MonoBehaviour
    {
        [SerializeField] private Image _fadeImage;
        [SerializeField] private float _fadeDuration = 0.5f;
        
        private bool _isFading;

        public async Task FadeOutAsync()
        {
            await StartFadeAsync(0f, 1f);
        }

        public async Task FadeInAsync()
        {
            await StartFadeAsync(1f, 0f);
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
    
    // @Action and Coroutine version
    
    // public class FadeController : MonoBehaviour
    // {
    //     [SerializeField] private Image _fadeImage;
    //     [SerializeField] private float _fadeDuration = 0.5f;
    //
    //     private Coroutine _fadeRoutine;
    //
    //     public void FadeIn(Action onComplete = null)
    //     {
    //         StartFade(1, 0, onComplete);
    //     }
    //
    //     public void FadeOut(Action onComplete = null)
    //     {
    //         StartFade(0, 1, onComplete);
    //     }
    //
    //     private void StartFade(float from, float to, Action onComplete)
    //     {
    //         if (_fadeRoutine != null)
    //             StopCoroutine(_fadeRoutine);
    //         _fadeRoutine = StartCoroutine(FadeRoutine(from, to, onComplete));
    //     }
    //
    //     private IEnumerator FadeRoutine(float from, float to, Action onComplete)
    //     {
    //         float time = 0f;
    //         Color color = _fadeImage.color;
    //
    //         while (time < _fadeDuration)
    //         {
    //             time += Time.deltaTime;
    //             float t = Mathf.Clamp01(time / _fadeDuration);
    //             color.a = Mathf.Lerp(from, to, t);
    //             _fadeImage.color = color;
    //             yield return null;
    //         }
    //
    //         color.a = to;
    //         _fadeImage.color = color;
    //         onComplete?.Invoke();
    //     }
    // }