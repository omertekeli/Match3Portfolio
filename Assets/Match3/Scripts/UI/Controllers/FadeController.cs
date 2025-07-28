using Cysharp.Threading.Tasks;
using DG.Tweening;
using Match3.Scripts.UI.Base;
using Match3.Scripts.UI.Views;

namespace Match3.Scripts.UI.Controllers
{
    public class FadeController : UIController<FadeView>
    {
        #region Fields
        private static bool _isFading;

        #endregion
        public FadeController(FadeView view) : base(view) { }
        internal async UniTask FadeToWhiteAsync(float? duration = null, Ease ease = Ease.InOutQuad)
        {
            await TryToFade(0f, duration, ease);
        }

        internal async UniTask FadeToBlackAsync(float? duration = null, Ease ease = Ease.InOutQuad)
        {
            await TryToFade(1f, duration, ease);
        }
        private async UniTask TryToFade(float targetAlpha, float? duration = null, Ease ease = Ease.InOutQuad)
        {
            if (_isFading) return;
            _isFading = true;
            await View.FadeToAlpha(targetAlpha, duration, ease);
            _isFading = false;
        }
    }
}