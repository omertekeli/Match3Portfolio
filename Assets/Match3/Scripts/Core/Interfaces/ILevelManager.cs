using Cysharp.Threading.Tasks;
using Match3.Scripts.Systems.Level.Base;
using System.Collections.Generic;

namespace Match3.Scripts.Core.Interfaces
{
    public interface ILevelManager
    {
        int RemainingMove { get; }
        IReadOnlyList<LevelGoalBase> LevelGoals { get; }

        UniTask PlayLevelIntroAnimationAsync();
        UniTask LoadAndSetupLevelAsync(int levelIndex);
    }
}