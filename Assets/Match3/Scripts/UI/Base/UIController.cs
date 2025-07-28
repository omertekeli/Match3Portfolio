namespace Match3.Scripts.UI.Base
{
    public abstract class UIController<T> where T : UIView
    {
        protected T View { get; private set; }
        protected UIController(T view)
        {
            this.View = view;
        }

        internal virtual void Show() => View.SetVisible(true);
        internal virtual void Hide() => View.SetVisible(false);
    }
}