using Match3.Scripts.Enums;

namespace Match3.Scripts.Systems.Board.Contents.Gem
{
    public class Gem : IBoardContent
    {
        public GemType Type { get; private set; }
        public Gem(GemType type)
        {
            Type = type;
        }

        public bool CanBeSwapped()
        {
            return true;
        }
    }
}