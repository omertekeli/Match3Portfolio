namespace Match3.Scripts.Core.Interfaces
{
    public interface IScoreSystem
    {
        int CurrentScore { get; }
        void Initialize();
        void Shutdown();
    }
}