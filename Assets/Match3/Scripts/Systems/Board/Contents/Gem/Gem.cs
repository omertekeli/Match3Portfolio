namespace Match3.Scripts.Systems.Board.Contents.Gem
{
    public class Gem
    {
        public GemData Data { get; private set; }
        public GemView View { get; private set; }

        public void Bind(GemData data, GemView view)
        {
            Data = data;
            View = view;
        }
    }
}