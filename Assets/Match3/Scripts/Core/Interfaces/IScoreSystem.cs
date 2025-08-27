namespace Match3.Scripts.Core.Interfaces
{
    public interface IScoreSystem : IService
    {
        int CurrentScore { get; }
        void Initialize();
        void Shutdown();
    }
}