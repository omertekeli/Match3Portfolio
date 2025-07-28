using Match3.Scripts.Enums;

namespace Match3.Scripts.Systems.Board.Contents.Gem
{
    public readonly struct GemData
    {
        public GemType Type { get; }
        public bool IsObjective { get; }

        public GemData(GemType type, bool isObjective = false)
        {
            Type = type;
            IsObjective = isObjective;
        }
    }
}