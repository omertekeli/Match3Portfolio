using JetBrains.Annotations;
using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Board.Data;

namespace Match3.Scripts.Systems.Board.Contents.Gem
{
    public class Gem : IBoardContent
    {
        public TileNode Node { get; set; }
        public GemType Type { get; private set; }
        public Gem(TileNode node, GemType type)
        {
            Type = type;
            Node = node;
        }

        public bool CanBeSwapped()
        {
            return true;
        }
    }
}