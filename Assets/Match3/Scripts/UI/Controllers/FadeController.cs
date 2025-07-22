using System.Threading.Tasks;
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
        internal async Task FadeToWhiteAsync() => await TryToFade(0f);
        internal async Task FadeToBlackAsync() => await TryToFade(1f);
        private async Task TryToFade(float targetAlpha)
        {
            if (_isFading) return;
            _isFading = true;
            await View.FadeToAlpha(targetAlpha);
            _isFading = false;
        }
    }
}