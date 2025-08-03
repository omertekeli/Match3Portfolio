using Cysharp.Threading.Tasks;
using Match3.Scripts.Systems.Level.Base;
using Match3.Scripts.Systems.Level.Data;
using System.Collections.Generic;

namespace Match3.Scripts.Core.Interfaces
{
    public interface ILevelManager
    {
        int RemainingMove { get; }
        IReadOnlyList<LevelGoalBase> LevelGoals { get; }
        UniTask LoadAndSetupLevelAsync(int levelIndex);
    }
}